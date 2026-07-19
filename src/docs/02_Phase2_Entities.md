# Phase 2 — Domain Entities & Database Schema

## الهدف
نمذجة كيانات المشروع كاملة وفق نمط جداول الترجمة المنفصلة (Translation Tables)، وربطها بقاعدة البيانات عبر EF Core Configurations، وإنشاء وتطبيق الـ Migration الأولى.

## النمط الأساسي
كل كيان ديناميكي متعدد اللغات يتكون من جدولين:
- **الجدول الأب** (بيانات لا تعتمد على اللغة: أرقام، صور، علاقات) يرث `BaseTranslatableEntity<TTranslation>`.
- **جدول الترجمة** (النصوص بكل لغة) يرث `BaseTranslation`، مع فهرس فريد `(ParentId, LanguageCode)` يمنع تكرار لغة لنفس السجل.
- القراءة عبر `GetTranslationWithFallback(lang)` — إن غابت الترجمة المطلوبة يعود للإنكليزية (اللغة الافتراضية).

## الكيانات

### كيانات مترجمة (جدول أب + جدول ترجمة)
| الكيان | حقول الأب | حقول الترجمة | ملاحظات |
|---|---|---|---|
| `City` | — | `Name` | فلتر مشترك للجامعات والسكن |
| `AcademicProgram` | — | `Name` | تخصصات (هندسة، طب، حقوق...) — Many-to-Many مع الجامعات |
| `University` | `Type` (Public/Private), `Rating`, `AnnualTuitionUsd`, `CampusCount`, `LogoUrl`, `CityId` | `Name`, `ShortDescription` | الكيان المحوري للمنصة |
| `Housing` | `PricePerMonthUsd`, `ImageUrl`, `CityId` | `Title`, `Description`, `Address` | صفحة السكن |
| `Faq` | `DisplayOrder` | `Question`, `Answer` | الأسئلة الشائعة |
| `Testimonial` | `PhotoUrl`, `DisplayOrder`, `IsPublished` | `StudentName`, `StudentTitle`, `QuoteText` | آراء الطلاب — ديناميكية بالكامل من لوحة التحكم |

### كيانات غير مترجمة (ترث `BaseEntity` فقط)
| الكيان | الحقول | ملاحظات |
|---|---|---|
| `Favorite` | `StudentUserId`, `UniversityId` | مفضلة الطالب — فهرس فريد على الزوج |
| `ConsultationRequest` | `StudentUserId`, `FullName`, `Phone`, `Message`, `UniversityId?`, `Status` | الاسم والهاتف لقطة (Snapshot) وقت الطلب |

### Enums جديدة
- `UniversityType`: Public / Private
- `ConsultationStatus`: New / Contacted / Closed

## قرار معماري: ربط Domain بجداول Identity
`Favorite` و`ConsultationRequest` يشيران للطالب بحقل `int StudentUserId` **بدون** Navigation Property إلى `ApplicationUser` — لأن طبقة `Domain` ممنوعة من الاعتماد على `Infrastructure` (قاعدة Clean Architecture). العلاقة الفعلية (FK) تُبنى في طبقة Infrastructure عبر:
```csharp
builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(f => f.StudentUserId);
```

## سلوكيات الحذف (Delete Behaviors)
| العلاقة | السلوك | السبب |
|---|---|---|
| أب → ترجماته | `Cascade` | الترجمة بلا معنى دون الأصل |
| `University.City` / `Housing.City` | `Restrict` | لا يمكن حذف مدينة فيها جامعات — يجب إعادة الإسناد أولاً |
| `Favorite.User` / `Favorite.University` | `Cascade` | المفضلة بيانات ثانوية |
| `ConsultationRequest.User` | `Restrict` | طلبات الاستشارة سجل تجاري (Leads) يجب أن يبقى حتى لو أُلغي الحساب — التعطيل عبر `IsActive` |
| `ConsultationRequest.University` | `SetNull` | حذف جامعة لا يمحو الطلبات المرتبطة بها |

## ملفات الـ Configuration
كل كيان له ملف Configuration في `Lumora.Infrastructure/Data/Configurations/` يحدد: اسم الجدول، أطوال الحقول (`HasMaxLength`)، الدقة العشرية (`decimal(3,2)` للتقييم، `decimal(10,2)` للأسعار)، الفهارس الفريدة، وسلوكيات الحذف. تُحمَّل جميعها تلقائياً عبر `ApplyConfigurationsFromAssembly`.

## أوامر الـ Migration المستخدمة
```bash
# إنشاء الـ Migration (من مجلد Lumora.Web)
dotnet ef migrations add InitialIdentityAndCoreEntities \
  --project ../Lumora.Infrastructure --startup-project . \
  --output-dir Data/Migrations

# تطبيقها على قاعدة البيانات
dotnet ef database update --project ../Lumora.Infrastructure --startup-project .
```

## الجداول الناتجة
- **7 جداول Identity** (`AspNetUsers`, `AspNetRoles`, `AspNetUserRoles`, ...)
- **15 جدول أعمال:** `Cities`, `CityTranslations`, `Universities`, `UniversityTranslations`, `AcademicPrograms`, `AcademicProgramTranslations`, `UniversityAcademicPrograms` (جدول الربط M:N), `Housings`, `HousingTranslations`, `Faqs`, `FaqTranslations`, `Testimonials`, `TestimonialTranslations`, `ConsultationRequests`, `Favorites`
