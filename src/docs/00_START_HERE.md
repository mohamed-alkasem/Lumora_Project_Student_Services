# Lumora — نقطة البداية للوكيل الجديد (START HERE)

> **اقرأ هذا الملف أولاً بالكامل قبل كتابة أي كود.** بعده اقرأ ملفات المراحل `01` → `04` بالترتيب لفهم التفاصيل.

آخر تحديث: 2026-07-18 — نهاية جلسة Phase 8 كاملة (السكن + الخدمات + من نحن + الأسئلة). كل الموقع العام واللوحة مكتملان؛ المتبقي Phase 9 (الشات) فقط.

**روابط الدخول السريعة (تشغيل محلي — بورت 5083 من launchSettings):**
- الموقع العام: `http://localhost:5083/Universities`
- لوحة الأدمن: `http://localhost:5083/Admin` — الحساب: `admin@lumora.local` / `Lumora@12345`

---

## 1. ما هو المشروع؟

**Lumora**: منصة ويب لخدمات الطلاب الجامعيين في تركيا. الميزات: دليل جامعات مع فلترة (حكومية/خاصة، مدينة، سقف رسوم، بحث، ترتيب)، سكن طلابي، طلبات استشارة، أسئلة شائعة، آراء طلاب (Testimonials)، مفضلة للطلاب، ولاحقاً شات مباشر.

- **التقنيات:** ASP.NET Core MVC (.NET 9) + EF Core 9 + SQL Server + Bootstrap 5 + jQuery. بدون Node.js.
- **المعمارية:** Clean Architecture بأربع طبقات في `d:\Lumora_Project\src`:
  - `Lumora.Domain` — الكيانات، Enums، واجهات المستودعات، أنواع الاستعلام. **صفر اعتماد خارجي.**
  - `Lumora.Application` — DTOs، الخدمات، الـ Mapping. تعتمد على Domain فقط (لا ترى EF Core).
  - `Lumora.Infrastructure` — DbContext، المستودعات، Identity، الـ Seeders. فيها `FrameworkReference` لـ `Microsoft.AspNetCore.App`.
  - `Lumora.Web` — MVC Controllers، Views، wwwroot، الترجمات، الـ DI الأخير.
- **5 لغات:** en (الافتراضية/الـ Fallback)، ar، tr، tk، fa — والعربية والفارسية RTL.
- **الهوية البصرية:** كحلي داكن (`#14294a`) + ذهبي/برتقالي (`#f2a51a`)، بطاقات، حواف دائرية. مصدر التصميم: ملفات PDF عند المالك (صفحة الجامعات إنكليزي وعربي).
- **لا يوجد Git repo حتى الآن** (المجلد ليس مستودعاً).

## 2. صاحب المشروع وأسلوب العمل معه

- يتواصل **بالعربية** (لهجة شامية) والمصطلحات التقنية بالإنكليزية — رُد عليه بنفس الأسلوب.
- يفوّض التنفيذ بالكامل لكن يريد عرض النتائج ومناقشتها بين المراحل، ويرحب بأسئلة التوضيح قبل القرارات الكبيرة.
- **قاعدة إلزامية:** بعد كل مرحلة أنشئ ملف توثيق مرقّم جديد في `docs/` (ممنوع ملف توثيق واحد كبير).
- **قاعدة إلزامية:** ممنوع أي CSS أو JS داخل ملفات `.cshtml` — ملفات معزولة لكل صفحة في `wwwroot/css/pages/` و `wwwroot/js/pages/` تُربط عبر `@section Styles` و `@section Scripts`.

## 3. بيئة التشغيل المحلية

```bash
# التشغيل (من مجلد Lumora.Web)
cd d:\Lumora_Project\src\Lumora.Web
dotnet run          # أو dotnet build ثم dotnet run --no-build

# الـ Migrations (دائماً من مجلد Lumora.Web)
dotnet ef migrations add <Name> --project ../Lumora.Infrastructure --startup-project . --output-dir Data/Migrations
dotnet ef database update --project ../Lumora.Infrastructure --startup-project .
```

- **قاعدة البيانات:** SQL Server محلي `DESKTOP-CNQ5IH7`، قاعدة `LumoraDb`، مستخدم `sa` (النص الكامل في `appsettings.Development.json`). الـ Migration الوحيدة `InitialIdentityAndCoreEntities` مطبّقة.
- **حساب الأدمن المزروع (dev فقط):** `admin@lumora.local` / `Lumora@12345` — يُزرع تلقائياً عند الإقلاع من قسم `SeedAdmin`. **يجب نقله لـ User Secrets قبل أي نشر.**
- **بيانات تجريبية:** `DevDataSeeder` يزرع 13 جامعة (7 حكومية/6 خاصة) + 3 مدن + 17 تخصصاً بترجمات en/ar/tr — يعمل مرة واحدة في بيئة Development فقط (يتخطى إذا وُجدت أي جامعة).
- أدوات: .NET SDK 10 مع `dotnet-ef` 10 كأداة Global.

## 4. حالة المراحل (أين وصلنا)

| المرحلة | الحالة | التوثيق |
|---|---|---|
| 1. Identity & Auth (أدوار SuperAdmin/Admin/Student، تسجيل عام للطلاب فقط، حماية Admin Area) | ✅ منجزة | `01_Phase1_Identity.md` |
| 2. الكيانات وقاعدة البيانات (نمط جداول الترجمة، 13 جدول أعمال + Identity) | ✅ منجزة | `02_Phase2_Entities.md` |
| 3. طبقة Application (DTOs، خدمات، فلترة/ترتيب/ترقيم داخل SQL) | ✅ منجزة | `03_Phase3_Application.md` |
| 4. واجهة دليل الجامعات العامة (View Components، Partials، AJAX، RTL) | ✅ منجزة ومختبرة | `04_Phase4_FrontEnd.md` |
| 5. Bundling & Minification عبر **WebOptimizer** | ✅ منجزة ومقاسة | `05_Phase5_Bundling.md` |
| 6. لوحة تحكم الأدمن (CRUD بتبويب 5 لغات، رفع صور، Users/Students/Consultations، حظر الموقوفين) | ✅ منجزة ومختبرة E2E | `06_Phase6_AdminDashboard.md` |
| 7. كماليات حساب الطالب: تفاصيل الجامعة، نموذج الاستشارة العام (مربوط بالأزرار)، صفحة المفضلة، الملف الشخصي وتغيير كلمة المرور، ترجمة أخطاء Identity | ✅ منجزة ومختبرة E2E | `07_Phase7_StudentAccount_Completions.md` |
| 8أ. صفحات السكن العامة (`/Housings` بفلاتر AJAX + `/Housings/Details/{id}`) | ✅ منجزة ومختبرة | `08_Phase8_HousingPages.md` |
| 8ب. صفحات Services / About+Testimonials / FAQ العامة — صفر روابط `#` متبقية | ✅ منجزة ومختبرة | `09_Phase8b_StaticPages.md` |
| 9. شات مباشر SignalR (طالب↔دعم، Inbox أدمن مع غير المقروء) + الصفحة الرئيسية من الـ PDF + فوتر 4 أعمدة ثابت + شريط عائم (واتساب/محادثة/مفضلة) + إعادة تصميم صفحات الحساب والاستشارة + تنظيم الداشبورد | ✅ منجزة ومختبرة E2E (شات عبر Long-Polling) | `10_Phase9_Chat_Landing_Polish.md` |

> **كل المراحل المخططة أُنجزت.** المتبقي أفكار تحسين فقط (انظر Backlog) — المالك يفضّل العمل "شوي شوي" ويهتم كثيراً بالجماليات والحركة (Animations). Migrations المطبقة: `InitialIdentityAndCoreEntities`, `AddChat`.

## 5. قرارات معمارية مُلزمة (لا تُناقض بدون موافقة المالك)

1. **الترجمة = جداول منفصلة:** كل كيان مترجم له جدول `XTranslations` بفهرس فريد `(XId, LanguageCode)`. القراءة عبر `GetTranslationWithFallback(lang)` (يسقط للإنكليزية).
2. **Domain لا يعرف Identity:** `Favorite.StudentUserId` و`ConsultationRequest.StudentUserId` أرقام `int` بلا Navigation — الـ FK يُبنى في Configurations الخاصة بالـ Infrastructure عبر `HasOne<ApplicationUser>()`.
3. **الفلترة داخل SQL:** واجهات مستودعات متخصصة (`IUniversityRepository.SearchAsync`...) مع أنواع استعلام في `Domain/Common` — ممنوع الفلترة/الترقيم في الذاكرة للقوائم الكبيرة.
4. **حذف ناعم (Soft delete)** عبر `IsDeleted` في كل الكيانات؛ الـ `GenericRepository` يفلتره تلقائياً. استثناء مهم: توجّل المفضلة يُحيي السجل المحذوف (بسبب الفهرس الفريد) — انظر `FavoriteService.ToggleAsync`.
5. **الصلاحيات Policy-based:** `AdminAccess` (SuperAdmin+Admin) و`SuperAdminOnly`. الحسابات الفرعية للأدمن كاملة الصلاحيات حالياً — التفصيل الدقيق (Granular) مؤجل بقرار المالك.
6. **حسابات الأدمن تُنشأ فقط من داخل اللوحة** بواسطة SuperAdmin (شاشة `Admin/Users` — دور Admin دائماً، لا حذف بل إيقاف عبر `IsActive`، وSuperAdmin لا يمكن إيقافه). التسجيل العام ينشئ **Student** حصراً، والحساب الموقوف يُمنع من الدخول بنفس رسالة كلمة المرور الخاطئة.
7. **التصفح عام؛ التفاعل يتطلب حساب طالب:** المفضلة وطلب الاستشارة والشات تتطلب دخول Student؛ زر القلب للزائر يحوّله لـ Login مع `returnUrl`.
8. **تخزين الصور محلياً** في `wwwroot/uploads/{subfolder}` خلف `IFileStorageService` (الواجهة في `Domain/Interfaces/Services` — لأن Infrastructure لا يشير إلى Application — والتنفيذ `Infrastructure/Storage/LocalFileStorageService`؛ أسماء ملفات GUID والحذف مقيّد داخل uploads).
9. **CSS بـ Logical Properties حصراً** (`margin-inline-start`...) — لا ملفات RTL منفصلة. الـ Layout يحمّل نسخة Bootstrap واحدة حسب الاتجاه.
10. **WebOptimizer** (حزمة NuGet) هي أداة الـ Bundling المعتمدة — وليس Node/Vite/Webpack.

## 6. ⚠️ مصائد معروفة (كلّفتنا وقتاً — لا تقع فيها)

1. **الترجمة (الأهم):** صف `SharedResources` يجب أن يبقى في الـ **namespace الجذري** `Lumora.Web` (ملفه في مجلد `Resources/` لكن الـ namespace جذري)، و`AddLocalization()` **بدون** `ResourcesPath`. السبب: اسم المورد المضمّن يتبع namespace الصف المجاور للـ resx. أي تغيير هنا يكسر الترجمة **بصمت** (تظهر أسماء المفاتيح كنصوص). كل الـ Views تستخدم `SharedLocalizer["Key"]` المحقون من `_ViewImports.cshtml` — **لا تستخدم `IViewLocalizer`** (لا توجد ملفات resx لكل View).
2. **Infrastructure كمكتبة صفوف** تحتاج `<FrameworkReference Include="Microsoft.AspNetCore.App" />` وإلا فشل `AddIdentity` بالبناء.
3. **إضافة لغة عربية/فارسية بمفاتيح resx:** ملفات ar/fa بصيغة block متعدد الأسطر، وtr/tk/en المضافة حديثاً بصيغة سطر واحد — حافظ على نمط كل ملف.
4. أزرار اللغة تمرر `returnUrl` = المسار الحالي؛ الكوكي `AspNetCore.Culture` هو المصدر الأول للثقافة ثم QueryString (`?culture=ar&ui-culture=ar` للاختبار بدون كوكي).
5. الاختبار بالـ `curl` على ويندوز: مخرجات عربية تكسر ترميز الكونسول — اكتب النتائج لملف UTF-8 واقرأه.
6. **اختبار بيئة Production محلياً:** `dotnet run` يفرض Development من `launchSettings.json` فوق متغير البيئة — استخدم `--no-launch-profile` مع تمرير `ConnectionStrings__DefaultConnection` (الجذر ما زال LocalDB placeholder).
7. **`sqlcmd` على قاعدة المشروع:** أي DELETE/UPDATE يفشل بخطأ QUOTED_IDENTIFIER بسبب الفهارس المفلترة — أضف العلم `-I` دائماً.
8. **عمليات قديمة تمسك البورت 5199:** قبل أي اختبار اقتل `Lumora.Web` (`powershell "Get-Process Lumora.Web | Stop-Process -Force"`) وإلا ستختبر نسخة قديمة بصمت.

## 7. أين الأشياء المهمة (خريطة سريعة)

- **إعداد الـ DI:** `Program.cs` → `AddInfrastructure()` (DB+Identity+Repos) و `AddApplicationServices()` (الخدمات الثماني).
- **الخدمات وواجهاتها:** `Lumora.Application/Interfaces/Services/IEntityServices.cs` (كل الواجهات بملف واحد) + `Services/` (التنفيذ) + `Services/Base/TranslatableCrudService.cs` (هيكل CRUD المشترك + `SyncTranslations`).
- **صفحة الجامعات:** `Controllers/UniversitiesController.cs` (`Index` كامل، `List` جزء AJAX) + `Views/Universities/` + `wwwroot/css/pages/universities.css` + `wwwroot/js/pages/universities.js`.
- **المفضلة:** `Controllers/FavoritesController.cs` (POST Toggle، JSON) — الزر بالبطاقات مربوط بالفعل.
- **الثيم المشترك:** `wwwroot/css/lumora-theme.css` (متغيرات `--lum-*` — استخدمها في أي صفحة جديدة).
- **Navbar/Footer:** `ViewComponents/` + `Views/Shared/Components/{Navbar,Footer}/Default.cshtml`. روابط Services/Housing/About حالياً `#` placeholders.
- **لوحة الأدمن (كاملة):** `Areas/Admin/` — Controllers لكل الكيانات + Consultations/Students/Users + Dashboard. الهيكل: `_AdminLayout.cshtml` (Sidebar من مصفوفة أقسام) + `wwwroot/css/admin.css` + `wwwroot/js/admin.js`. نمط CRUD: `Index/Create/Edit + _Form.cshtml` مع `_LangTabsHeader` partial و`TranslationForm.Normalize` للتبويبات الخمس. رفع الصور: `IFormFile?` منفصل عن الـ DTO + `AdminUpload.Validate`.
- **Bundling:** إعداد WebOptimizer في `Program.cs` (Development = خام، غيره = تجميع/تصغير). لاختبار الإنتاج محلياً: `--no-launch-profile` ضروري (انظر `05_Phase5_Bundling.md`).

## 8. كيف تبدأ الجلسة القادمة (خطوات عملية)

1. اقرأ هذا الملف + ملف المرحلة التي ستعمل عليها.
2. شغّل `dotnet build` من `Lumora.Web` للتأكد أن كل شيء أخضر (آخر حالة: 0 أخطاء 0 تحذيرات).
3. شغّل التطبيق وافتح `/Universities` (الصفحة العامة) و`/Admin` بحساب `admin@lumora.local` للتأكد من عمل كل شيء.
4. اسأل المالك: أي مرحلة نبدأ؟ (المرشح الطبيعي: Phase 8 الصفحات العامة — أو Phase 7 الشات).
5. عند الإنجاز: ملف توثيق جديد `docs/07_...md` بنفس نمط الملفات السابقة (عربي، جداول ملفات، قرارات، أوامر، نتائج اختبار).

## 9. أشياء معلّقة صغيرة (Backlog دقيق)

- [ ] `Home/Index` ما زالت صفحة Placeholder (عنوان ترحيبي فقط) — تحتاج تصميم Landing لاحقاً.
- [ ] الترجمات التركمانية والفارسية للبيانات التجريبية غير مزروعة (تسقط للإنكليزية) — مقبول للتطوير.
- [ ] لا يوجد ترقيم صفحات بقوائم الأدمن عدا Universities وConsultations — يلزم لاحقاً إذا كبرت الجداول (خصوصاً Students).
- [ ] الفهرس الفريد على المفضلة لا يمنع تكرار طلبات الاستشارة (مقصود — الطالب قد يطلب أكثر من مرة).
- [ ] صفحة تفاصيل الجامعة لا تعرض زر القلب حالياً (القلب في البطاقات فقط) — تحسين صغير إن طُلب.
- [ ] "نسيت كلمة المرور" (Forgot password) غير موجودة — تحتاج بريداً صادراً (SMTP) غير مُعدّ بعد.
