# Phase 3 — Application Layer (DTOs & Services)

## الهدف
بناء طبقة الـ Application كاملة: DTOs لكل كيان، خدمات (Services) بمنطق الفلترة (النوع/المدينة/سقف الرسوم)، الترتيب (الأعلى تقييماً/السعر/الأحدث)، والـ Pagination — مع بقاء الطبقة نظيفة تماماً من أي اعتماد على EF Core.

## القرار المعماري المحوري
الفلترة والترقيم يجب أن يحدثا **داخل SQL** وليس في الذاكرة، لكن `Application` لا ترى EF Core. الحل:
1. أنواع الاستعلام (`UniversityQueryParameters`, `PagedResult<T>`...) تعيش في **`Domain/Common`** كأصناف C# نقية.
2. واجهات مستودعات متخصصة (`IUniversityRepository`...) في **`Domain/Interfaces`** تعلن عن `SearchAsync`.
3. التنفيذ الفعلي بـ LINQ-to-SQL في **`Infrastructure/Repositories`**.
4. خدمات **`Application`** تستدعي الواجهة وتحوّل النتيجة إلى DTOs — دون معرفة أي شيء عن EF.

## الملفات الجديدة

### Lumora.Domain
| الملف | الوصف |
|---|---|
| `Common/PagedResult.cs` | نتيجة مرقّمة عامة: `Items`, `TotalCount`, `TotalPages`, `HasNext/HasPrevious` + دالة `Map<TOut>` للتحويل إلى DTO دون فقدان بيانات الترقيم |
| `Common/UniversityQueryParameters.cs` | فلاتر الجامعات: بحث، نوع، مدينة، سقف رسوم، ترتيب — مع تحصين `Page ≥ 1` و`PageSize ≤ 50` |
| `Common/HousingQueryParameters.cs` | فلاتر السكن: مدينة + سقف سعر شهري |
| `Common/ConsultationQueryParameters.cs` | فلتر صندوق الاستشارات: الحالة |
| `Enums/UniversitySortOption.cs` | `TopRated` / `TuitionAsc` / `TuitionDesc` / `Newest` |
| `Interfaces/Repositories/IUniversityRepository.cs` | `SearchAsync`, `GetWithDetailsAsync`, `GetByIdsWithDetailsAsync` (للمفضلة) |
| `Interfaces/Repositories/IHousingRepository.cs` | `SearchAsync`, `GetWithDetailsAsync` |
| `Interfaces/Repositories/IConsultationRepository.cs` | `GetPagedAsync` لصندوق وارد الأدمن |
| `Interfaces/Repositories/IFavoriteRepository.cs` | `GetIncludingDeletedAsync` — ضروري لمنطق الـ Toggle (انظر أدناه) |

### Lumora.Infrastructure
| الملف | الوصف |
|---|---|
| `Repositories/UniversityRepository.cs` | الفلترة داخل SQL، البحث يطابق اسم الجامعة **أو التخصص بأي لغة**، الترتيب حسب `UniversitySortOption`، ثم `Skip/Take` + `AsSplitQuery()` لتفادي انفجار الـ JOIN |
| `Repositories/HousingRepository.cs` | نفس النمط للسكن |
| `Repositories/ConsultationRepository.cs` | ترقيم بالأحدث أولاً مع تحميل اسم الجامعة المرتبطة |
| `Repositories/FavoriteRepository.cs` | البحث شاملاً السجلات المحذوفة ناعماً |
| `Extensions/InfrastructureServiceExtensions.cs` | تسجيل المستودعات الأربعة الجديدة في DI |

### Lumora.Application
| الملف | الوصف |
|---|---|
| `DTOs/Cities/CityDtos.cs` | `CityDto` + Create/Update مع قائمة ترجمات |
| `DTOs/Programs/AcademicProgramDtos.cs` | نفس النمط |
| `DTOs/Universities/UniversityDtos.cs` | `UniversityCardDto` (بطاقة الدليل)، `UniversityDetailsDto`، `UniversityFilterDto` (يُربط من الـ QueryString)، Create/Update مع `ProgramIds` |
| `DTOs/Housings/HousingDtos.cs` | Card / Details / Filter / Create / Update |
| `DTOs/Faqs/FaqDtos.cs` + `DTOs/Testimonials/TestimonialDtos.cs` | نفس النمط |
| `DTOs/Consultations/ConsultationDtos.cs` | `ConsultationCreateDto` (من الطالب) + `ConsultationAdminDto` (صندوق الوارد) |
| `Mapping/EntityMappings.cs` | Extension methods مشتركة (`ToCardDto`, `ToDetailsDto`) — تُستخدم من أكثر من خدمة فلا تُكرر |
| `Services/Base/TranslatableCrudService.cs` | **الهيكل العام للـ CRUD**: خمس عمليات + `SyncTranslations` (إضافة/تعديل/حذف الترجمات دفعة واحدة) — كل خدمة تكتب فقط دوال الـ Mapping الخاصة بها |
| `Services/CityService.cs`, `AcademicProgramService.cs`, `FaqService.cs`, `TestimonialService.cs` | ترث الـ Base وتضيف ما يلزم (`GetOrderedAsync`, `GetPublishedAsync`) |
| `Services/UniversityService.cs` | `SearchAsync` + Override للـ Create/Update لمزامنة علاقة M:N مع التخصصات |
| `Services/HousingService.cs` | `SearchAsync` بفلاتر السكن |
| `Services/ConsultationService.cs` | إنشاء طلب (مع Trim + Snapshot)، صندوق وارد مرقّم، تحديث الحالة |
| `Services/FavoriteService.cs` | Toggle + قائمة مفضلة الطالب + `GetMyFavoriteIdsAsync` لتلوين القلوب في الدليل |
| `Interfaces/Services/IEntityServices.cs` | واجهات الخدمات الثماني |

### Lumora.Web
| الملف | الوصف |
|---|---|
| `Extensions/ApplicationServiceExtensions.cs` | تسجيل الخدمات الثماني في DI |
| `appsettings.Development.json` | نص الاتصال المحلي الجديد (`DESKTOP-CNQ5IH7` / SQL Auth) |

## منطق Toggle المفضلة (لماذا `GetIncludingDeletedAsync`؟)
الفهرس الفريد `(StudentUserId, UniversityId)` يشمل السجلات المحذوفة ناعماً (`IsDeleted = true`). لو حذف الطالب جامعة من مفضلته ثم أعادها، إدراج سجل جديد سيصطدم بالفهرس. لذلك الـ Toggle:
- لا سجل → إدراج جديد.
- سجل موجود → قلب قيمة `IsDeleted` (إحياء أو إخفاء).

## مثال تدفق كامل (بحث الجامعات)
```
GET /Universities?Type=Private&CityId=1&MaxAnnualTuitionUsd=10000&SortBy=TopRated&Page=2
        ↓
UniversityFilterDto  (Web — Model Binding من QueryString)
        ↓
IUniversityService.SearchAsync(filter, "ar")   (Application)
        ↓
IUniversityRepository.SearchAsync(parameters)  (Domain interface)
        ↓
SQL: WHERE Type=1 AND CityId=1 AND Tuition<=10000
     ORDER BY Rating DESC  OFFSET 12 ROWS FETCH NEXT 12
        ↓
PagedResult<University> → .Map(u => u.ToCardDto("ar")) → PagedResult<UniversityCardDto>
```

## التحقق
- بناء ناجح: 0 أخطاء، 0 تحذيرات.
- الـ Migration طُبقت بنجاح على `DESKTOP-CNQ5IH7` (قاعدة `LumoraDb` أُنشئت كاملة).
- التطبيق يقلع على السيرفر الجديد: `/` و`/Account/Login` تعيدان 200، والزرع (Seeding) يعمل.

## ملاحظات للمرحلة القادمة (Phase 4)
- الكنترولرز العامة ستستهلك `IUniversityService.SearchAsync` مباشرة مع تمرير لغة الطلب الحالية.
- `GetMyFavoriteIdsAsync` جاهزة لتلوين أيقونات القلب في بطاقات الدليل للطالب المسجل.
- شاشات لوحة التحكم (Phase 6) ستستخدم Create/Update DTOs الجاهزة مع تبويب للغات الخمس.
