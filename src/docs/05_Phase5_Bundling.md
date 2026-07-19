# Phase 5 — Bundling & Minification (WebOptimizer)

## الهدف
تفعيل تجميع وضغط ملفات CSS/JS للإنتاج عبر **LigerShark.WebOptimizer.Core** (حزمة NuGet خالصة — بدون Node.js أو خطوة build خارجية)، مع إبقاء بيئة التطوير على الملفات الخام لسهولة التصحيح.

## ما تم

### 1. الحزمة
```bash
dotnet add package LigerShark.WebOptimizer.Core   # (من مجلد Lumora.Web)
```

### 2. الإعداد في `Program.cs`
- **Development:** `AddWebOptimizer(minifyJavaScript: false, minifyCss: false)` — لا تصغير ولا تجميع؛ الملفات الخام تُخدم كما هي.
- **غير Development:**
  - `AddCssBundle("/css/shared.min.css", "css/lumora-theme.css", "css/site.css")` — حزمة الثيم المشترك.
  - `AddJavaScriptBundle("/js/shared.min.js", "js/site.js")`.
  - `MinifyCssFiles("css/pages/**/*.css", "css/admin.css")` و `MinifyJsFiles("js/pages/**/*.js", "js/admin.js")` — ملفات الصفحات تُصغَّر **بنفس مساراتها** (لا حاجة لتغيير أي View، وتبقى فلسفة "ملف لكل صفحة" كما هي).
- `app.UseWebOptimizer()` **قبل** `UseStaticFiles()` (إلزامي — وإلا تُخدم الملفات الخام دائماً).

### 3. الـ Layouts
`_Layout.cshtml` (والأدمن لاحقاً) يستخدم `<environment>` Tag Helpers: ملفات خام منفصلة في Development، وحزمة `shared.min.css`/`shared.min.js` خارجه.

## مصائد واجهناها (مهم للوكيل القادم)
1. **`dotnet run` يتجاهل متغير البيئة**: `launchSettings.json` يفرض `ASPNETCORE_ENVIRONMENT=Development` فوق متغيرات البيئة. لاختبار الإنتاج محلياً استخدم `--no-launch-profile`:
   ```bash
   ASPNETCORE_ENVIRONMENT=Production ConnectionStrings__DefaultConnection="<conn>" \
     dotnet run --no-build --no-launch-profile --urls http://localhost:5199
   ```
   (تمرير الـ connection ضروري لأن `appsettings.json` الجذر ما زال يشير لـ LocalDB placeholder.)
2. **ملف JS كله تعليقات = 404**: كان `site.js` تعليقات فقط، فالمصغّر أنتج ملفاً فارغاً وWebOptimizer أعاد 404 للحزمة. وُضع محتوى فعلي (إضافة صنف `js-enabled` على `<html>`). لا تُنشئ حزمة من ملفات فارغة.
3. **Overload resolution**: استخدم `AddWebOptimizer((WebOptimizer.IAssetPipeline pipeline) => ...)` بالنوع الصريح، والاسم الصحيح `AddJavaScriptBundle` (لا يوجد `AddJsBundle`).
4. **عمليات قديمة تمسك البورت**: عند اختبار متكرر على نفس البورت، اقتل `Lumora.Web` أولاً (`Get-Process Lumora.Web | Stop-Process -Force`) وإلا ستختبر نسخة قديمة دون أن تدري.

## نتائج القياس (Production فعلياً)
| الملف | خام | مُصغّر |
|---|---|---|
| `shared.min.css` (theme+site مجمّعة) | ~9.4KB | **3,970B** |
| `css/pages/universities.css` | 12,028B | **8,858B** |
| `js/pages/universities.js` | 8,123B | **3,769B** |
| `shared.min.js` | — | 81B |

كل الحزم تُخدم 200 مع Cache headers من WebOptimizer، و`/Universities` بالإنتاج يشير لـ `shared.min.css` بدل الملفات المنفصلة.
