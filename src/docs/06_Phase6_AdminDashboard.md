# Phase 6 — Admin Dashboard (لوحة تحكم الإدارة)

## الهدف
لوحة تحكم كاملة بهوية Lumora: CRUD لكل المحتوى الديناميكي بتبويب اللغات الخمس، رفع صور محلي خلف واجهة قابلة للتبديل، صندوق طلبات الاستشارة، قائمة الطلاب، وإدارة حسابات الأدمن (SuperAdmin فقط) — مع حظر الحسابات الموقوفة من الدخول.

## 1. توسعة طبقة Application
- **`ITranslatableService.GetForEditAsync(id)`**: نموذج التعديل يحتاج الكيان **بكل ترجماته** وليس إسقاطاً بلغة واحدة. أضيفت للقاعدة `TranslatableCrudService` مع `MapToUpdateDto` abstract نفّذتها الخدمات الست (Cities, Programs, Faqs, Testimonials, Housings, Universities).
- `UniversityService` تتجاوز `GetForEditAsync` لتستخدم `GetWithDetailsAsync` (لأن النموذج يحتاج `ProgramIds` أيضاً).

## 2. تخزين الملفات
- الواجهة: `Lumora.Domain/Interfaces/Services/IFileStorageService.cs` (في Domain اتباعاً لاصطلاح المشروع: التجريدات في Domain لأن Infrastructure لا يشير إلى Application).
  - `SaveAsync(Stream, originalFileName, subfolder)` → ترجع URL نسبي مثل `/uploads/universities/{guid}.png`
  - `DeleteAsync(relativeUrl)` — تتجاهل الملفات غير الموجودة.
- التنفيذ: `Infrastructure/Storage/LocalFileStorageService.cs` — تخزين في `wwwroot/uploads/{subfolder}`، اسم ملف GUID (لا يُستخدم اسم المستخدم أبداً)، والحذف مقيّد داخل مجلد uploads فقط (حماية من path traversal). مسجّلة Singleton في `AddInfrastructure`.
- التحقق في `Areas/Admin/Models/AdminUpload.cs`: امتدادات `.png .jpg .jpeg .webp .svg` وحد أقصى 2MB — برسائل مترجمة (`ImageTooLarge`, `ImageTypeNotAllowed`).

## 3. هيكل اللوحة (Shell)
- `_AdminLayout.cshtml` أُعيد بناؤه بالكامل: Sidebar كحلي بروابط مبنية من مصفوفة (controller, resourceKey) — إضافة قسم جديد = سطر واحد؛ رابط Users يظهر للـ SuperAdmin فقط؛ Topbar فيه أزرار اللغات الخمس + اسم المستخدم + خروج؛ عرض `TempData["Success"/"Error"]` كـ Alerts. (أُصلح تحميل Bootstrap المزدوج القديم.)
- `wwwroot/css/admin.css` (جديد): يبني على متغيرات `lumora-theme.css`، Logical Properties بالكامل، فيه: shell/sidebar/topbar، جداول admin-table، بطاقات إحصائيات، Badges للحالات، تبويبات اللغات.
- `wwwroot/js/admin.js` (جديد): تأكيد الحذف عبر `form[data-confirm]`، وتفعيل التاب الذي فيه أخطاء Validation تلقائياً بعد فشل الحفظ (حتى لا "تختفي" الأخطاء بتبويب غير نشط). كلاهما مضاف لتصغير WebOptimizer.
- `Areas/Admin/Views/_ViewImports.cshtml`: أضيف حقن `SharedLocalizer` و`@using Lumora.Domain.Constants`.
- `AdminBaseController`: أصبح المسار `Admin/[controller]/[action]/{id?}`.

## 4. شاشات CRUD (النمط الموحد)
لكل كيان: `Index` (جدول) + `Create` + `Edit` + `Delete` (POST + تأكيد). النموذج partial واحد `_Form.cshtml` يُستدعى من Create/Edit.
- **تبويب اللغات**: partial مشترك `Areas/Admin/Views/Shared/_LangTabsHeader.cshtml` يرسم رؤوس التبويبات، وكل نموذج يرسم `tab-pane` لكل لغة فيها حقولها مع `dir` صحيح لكل لغة (تكتب العربية RTL داخل تبويبها حتى لو واجهة الأدمن إنكليزية). الربط عبر `Translations[i].LanguageCode` (hidden) + الحقول، والمساعد `TranslationForm.Normalize<T>()` يضمن دائماً 5 عناصر بترتيب `LanguageConstants.All`.
- **الكيانات**: Cities وPrograms (اسم فقط)، Faqs (سؤال/جواب + ترتيب)، Testimonials (اسم/صفة/رأي + صورة + ترتيب + نشر)، Housings (عنوان/وصف/عنوان تفصيلي + مدينة + سعر + صورة)، Universities (كل الحقول + مدينة + تخصصات Checkboxes + شعار).
- **رفع الصور**: الأكشن يستقبل `IFormFile?` منفصلاً عن الـ DTO (الـ DTO في Application يبقى نقياً بلا أنواع ASP.NET). عند رفع ملف جديد يُحذف القديم؛ وعند حذف الكيان تُحذف صورته.

## 5. الشاشات الإدارية
- **Consultations**: فلترة بالحالة (الكل/جديد/تم التواصل/مغلق) + أزرار تغيير حالة مباشرة من الصف، مع الحفاظ على الفلتر الحالي بعد التغيير.
- **Students**: قائمة كل مستخدمي دور Student (الاسم، البريد، تاريخ التسجيل، الحالة).
- **Users** (`SuperAdminOnly` policy): قائمة SuperAdmin/Admin، إنشاء Sub-Admin (دور Admin دائماً)، تفعيل/إيقاف عبر `IsActive` — لا يمكن إيقاف أي SuperAdmin، ولا يوجد حذف (بقرار: الإيقاف يكفي ويحفظ التاريخ).
- **Dashboard**: بطاقات إحصائية (جامعات/طلاب/سكن/مدن/تخصصات/أسئلة/آراء + الاستشارات الجديدة تتلوّن أحمر إذا > 0) كلها روابط لأقسامها.

## 6. حظر الحسابات الموقوفة
`AccountController.Login`: قبل محاولة الدخول، إذا `IsActive == false` تُرفض المحاولة **بنفس رسالة** كلمة المرور الخاطئة (لا نكشف أن الحساب موجود وموقوف).

## 7. الترجمة
أضيف ~50 مفتاحاً × 5 لغات (كل عناوين اللوحة والحقول والحالات ورسائل الرفع) — بسكربت Python يحقن قبل `</root>` ويحافظ على نمط كل ملف (en/ar متعدد الأسطر، tr/tk/fa سطر واحد).

## 8. الاختبار (E2E فعلي عبر curl مع كوكيز وAnti-Forgery)
| السيناريو | النتيجة |
|---|---|
| دخول SuperAdmin ثم فتح كل الشاشات الـ 12 | 200 كلها |
| إنشاء مدينة "Bursa" بخمس ترجمات عبر POST | 302 → ظهرت بقائمة الأدمن **وفوراً كـ Chip بفلاتر الصفحة العامة** |
| نموذج التعديل يعود معبّأً بالترجمات | ✓ |
| حذف المدينة (soft delete) | ✓ اختفت من القائمة |
| إنشاء Sub-Admin من شاشة Users | ✓ يدخل للوحة، و**يُمنع** من شاشة Users (302) |
| إيقاف الـ Sub-Admin ثم محاولة دخوله | صفحة الدخول 200 مع "Invalid email or password" ✓ |
| بيانات الاختبار نُظّفت | المدينة بـ soft-delete من اللوحة، والحساب حُذف بـ sqlcmd (استخدم `-I` لتفعيل QUOTED_IDENTIFIER وإلا يفشل DELETE على جداول بفهارس مفلترة) |

## ملاحظات للوكيل القادم
- لإنشاء أدمن جديد أو أي إجراء بالـ curl: الفورمات كلها تحتاج `__RequestVerificationToken` من نفس الصفحة + كوكيز الجلسة.
- رسائل أخطاء Identity (مثل قواعد كلمة المرور) لا تزال إنكليزية — `IdentityErrorDescriber` مترجم = تحسين مؤجل.
- الشاشات لا تحتوي ترقيماً إلا Universities (20/صفحة) وConsultations (20/صفحة) — الباقي قوائم كاملة (مقبول لأحجامها).
