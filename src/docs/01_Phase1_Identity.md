# Phase 1 — Identity & Authentication

## الهدف
تأسيس نظام الهوية والصلاحيات: ثلاثة أدوار (SuperAdmin / Admin / Student)، تسجيل عام للطلاب فقط، حماية منطقة الأدمن، وزرع (Seed) الأدوار وحساب SuperAdmin الأول تلقائياً.

## القرارات المعمارية
| القرار | التفصيل |
|---|---|
| نوع المفتاح | `IdentityUser<int>` — مفاتيح `int` بدل GUID لتتوافق مع بقية الكيانات |
| مكان Identity | طبقة `Infrastructure` (وليس `Domain`) لأن Identity تقنية وليست منطق عمل |
| التسجيل العام | ينشئ حسابات **Student فقط** — لا يوجد أي مسار عام لإنشاء أدمن |
| حسابات الأدمن | تُنشأ حصرياً من داخل لوحة التحكم بواسطة SuperAdmin (ستُبنى شاشتها في Phase 6) |
| الصلاحيات | Policy-based: `AdminAccess` (SuperAdmin+Admin) و`SuperAdminOnly` — قابلة للتفصيل لاحقاً دون هدم |

## الملفات المُنشأة / المعدّلة

### Lumora.Domain
| الملف | الوصف |
|---|---|
| `Constants/RoleConstants.cs` | أسماء الأدوار الثلاثة + مصفوفة `AdminRoles` المسموح لها بدخول لوحة التحكم |

### Lumora.Infrastructure
| الملف | الوصف |
|---|---|
| `Identity/ApplicationUser.cs` | يوسّع `IdentityUser<int>` بـ: `FullName`, `CreatedAt`, `IsActive` (لتعطيل حساب دون حذفه) |
| `Identity/IdentitySeeder.cs` | يزرع الأدوار الثلاثة + أول SuperAdmin من إعدادات `SeedAdmin` عند إقلاع التطبيق |
| `Data/ApplicationDbContext.cs` | تحوّل من `DbContext` إلى `IdentityDbContext<ApplicationUser, IdentityRole<int>, int>` |
| `Extensions/InfrastructureServiceExtensions.cs` | إضافة `AddIdentity` مع سياسة كلمة مرور (8 أحرف كحد أدنى، بريد فريد) |
| `Lumora.Infrastructure.csproj` | حزمة `Microsoft.AspNetCore.Identity.EntityFrameworkCore` + **`FrameworkReference` إلى `Microsoft.AspNetCore.App`** |

> **ملاحظة مهمة (FrameworkReference):** مشروع `Infrastructure` مكتبة صفوف عادية (`Microsoft.NET.Sdk`)، بينما `UserManager`/`SignInManager`/`AddIdentity` تعيش في الـ Shared Framework الذي لا يصل تلقائياً إلا لمشاريع `Microsoft.NET.Sdk.Web`. بدون سطر `<FrameworkReference Include="Microsoft.AspNetCore.App" />` يفشل البناء بخطأ `CS1061: AddIdentity not found`.

### Lumora.Web
| الملف | الوصف |
|---|---|
| `Program.cs` | `ConfigureApplicationCookie` (مسارات Login/Logout/AccessDenied)، سياسات `AdminAccess`/`SuperAdminOnly`، إضافة `UseAuthentication()` قبل `UseAuthorization()`، واستدعاء `IdentitySeeder.SeedAsync` عند الإقلاع |
| `Controllers/AccountController.cs` | Login / Register (طالب فقط + `lockoutOnFailure`) / Logout (POST) / AccessDenied — مع `RedirectToLocal` ضد Open-Redirect |
| `Models/Account/LoginViewModel.cs` | نموذج الدخول مع مفاتيح أخطاء قابلة للترجمة |
| `Models/Account/RegisterViewModel.cs` | نموذج التسجيل (FullName, Email, Password, ConfirmPassword) |
| `Views/Account/Login.cshtml` + `Register.cshtml` + `AccessDenied.cshtml` | واجهات متوافقة مع RTL/LTR ومترجمة بالكامل |
| `Views/Shared/_Layout.cshtml` | الـ Navbar يعرض دخول/تسجيل أو اسم المستخدم/خروج حسب حالة المصادقة |
| `Areas/Admin/Controllers/AdminBaseController.cs` | إضافة `[Authorize(Policy = "AdminAccess")]` — كل كنترولر أدمن يرث الحماية تلقائياً |
| `Resources/SharedResources.*.resx` | 16 مفتاح ترجمة جديد للـ Auth بكل اللغات الخمس (en, ar, tr, tk, fa) |
| `appsettings.Development.json` | قسم `SeedAdmin` (بريد + كلمة مرور الحساب الأول — بيئة التطوير فقط) |

## بيانات الدخول الافتراضية (تطوير فقط)
```
Email:    admin@lumora.local
Password: Lumora@12345
```
> **قبل النشر:** يجب نقل هذه القيم إلى User Secrets أو متغيرات البيئة وتغيير كلمة المرور.

## التحقق
- بناء ناجح بدون تحذيرات.
- `/` و`/Account/Login` و`/Account/Register` تعيد **200**.
- `/Admin/Home/Index` بدون تسجيل دخول تعيد **302** إلى صفحة الدخول (الحماية فعّالة).
- الأدوار الثلاثة + حساب SuperAdmin تُزرع تلقائياً عند أول إقلاع.
