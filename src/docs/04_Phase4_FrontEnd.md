# Phase 4 — Public Front-End (University Directory)

## الهدف
بناء صفحة دليل الجامعات العامة بمطابقة الهوية البصرية المعتمدة (كحلي داكن + ذهبي/برتقالي، بطاقات، فلاتر جانبية)، مع تجزئة كاملة للمكونات، وعزل تام لملفات CSS/JS، ودعم RTL/LTR تلقائي، وفلترة ومفضلة ديناميكية عبر AJAX.

## المكونات المُنشأة

### View Components (المكونات المشتركة بين كل الصفحات)
| المكون | الملفات | الوصف |
|---|---|---|
| Navbar | `ViewComponents/NavbarViewComponent.cs` + `Views/Shared/Components/Navbar/Default.cshtml` | شريطان: Topbar كحلي (اسم الدليل + أزرار اللغات الخمس بأسمائها الأصلية Native names) وNavbar أبيض (شعار LUM●RA، روابط التنقل مع حالة Active، زر "استشارة مجانية" الذهبي، ودخول/خروج حسب حالة المستخدم) |
| Footer | `ViewComponents/FooterViewComponent.cs` + `Views/Shared/Components/Footer/Default.cshtml` | فوتر كحلي بالشعار والروابط وحقوق النشر |

### Partial Views (مكونات صفحة الجامعات)
| الملف | الوصف |
|---|---|
| `Views/Universities/_FiltersSidebar.cshtml` | بطاقة الفلاتر: نوع الجامعة مع العدّادات (الكل/حكومية/خاصة)، رقائق المدن (Chips)، شريط تمرير سقف الرسوم ($500–$25,000+)، وعدّاد المفضلة |
| `Views/Universities/_UniversityList.cshtml` | **الجزء القابل للاستبدال عبر AJAX**: الشبكة + الترقيم + حالة "لا نتائج"، ويحمل `data-total` لتحديث العدّاد |
| `Views/Universities/_UniversityCard.cshtml` | البطاقة: شريط علوي ملوّن (كحلي=حكومية بأيقونة مبنى كلاسيكي / ذهبي=خاصة بأيقونة برج)، شارة "★ الأفضل" للتقييم ≥ 4.8، زر القلب، الشارات والتقييم، الموقع وعدد الفروع، رقائق التخصصات، الرسوم السنوية، زر "قدّم الآن" |

### Controllers & ViewModels
| الملف | الوصف |
|---|---|
| `Controllers/UniversitiesController.cs` | `Index` (الصفحة الكاملة) و`List` (يعيد الـ Partial فقط للـ AJAX) — كلاهما يستقبل `UniversityFilterDto` من الـ QueryString مباشرة، فالروابط قابلة للمشاركة وتعمل بدون JS |
| `Controllers/FavoritesController.cs` | `Toggle` (POST + AntiForgery + `[Authorize(Roles=Student)]`) يعيد JSON بالحالة الجديدة والعدد |
| `Models/Universities/UniversityPageViewModels.cs` | `UniversityIndexViewModel` / `UniversityListViewModel` / `UniversityCardViewModel` |

### ملفات الواجهة المعزولة (قاعدة: صفر CSS/JS داخل cshtml)
| الملف | الوصف |
|---|---|
| `wwwroot/css/lumora-theme.css` | **الثيم المشترك**: متغيرات CSS للهوية (`--lum-navy-900`, `--lum-gold`...)، Topbar، Navbar، الأزرار، Footer |
| `wwwroot/css/pages/universities.css` | كل تنسيقات الصفحة (Hero، البحث، الفلاتر، الشبكة، البطاقات، الترقيم) — مربوط عبر `@section Styles` |
| `wwwroot/js/pages/universities.js` | منطق الصفحة كاملاً — مربوط عبر `@section Scripts`، يقرأ الـ endpoints وحالة الدخول من `data-*` على `#uniPage` (صفر inline JS) |

## دعم RTL/LTR
- **CSS Logical Properties حصراً**: `margin-inline-start`, `inset-inline-end`, `padding-block`, `border-end-start-radius`, `text-align: start`... — التصميم ينعكس تلقائياً مع `dir="rtl"` دون ملف RTL منفصل.
- الـ Layout يحمّل الآن **نسخة Bootstrap واحدة** حسب الاتجاه (`bootstrap.rtl.min.css` للعربية/الفارسية فقط) — سابقاً كانت النسختان تُحمّلان معاً.
- توهّج الـ Hero الذهبي موضوع بـ `inset-inline-end` فينتقل للجهة الصحيحة تلقائياً بالعربية.

## الفلترة الديناميكية (AJAX)
- كل تغيير فلتر يستدعي `GET /Universities/List?...` ويستبدل `#uniList` فقط — **بدون إعادة تحميل الصفحة**.
- بحث مع Debounce (300ms) + حارس تسلسل (Sequence guard) يتجاهل الاستجابات المتأخرة القديمة.
- تحديث عنوان المتصفح بـ `history.replaceState` — الرابط الحالي يعكس الفلاتر دائماً وقابل للمشاركة.
- زر Reset يعيد كل شيء للوضع الافتراضي.
- **البحث عابر اللغات**: كتابة "طب" بالعربية وأنت على الواجهة الإنكليزية تجد الجامعات، لأن البحث في SQL يطابق كل جداول الترجمة (اسم الجامعة أو التخصص بأي لغة).

## المفضلة (القلب)
- زائر غير مسجل → الضغط على القلب يحوّله لصفحة الدخول مع `returnUrl` يعيده لنفس الصفحة بنفس الفلاتر.
- طالب مسجل → `POST /Favorites/Toggle` مع Anti-Forgery Token بالهيدر، وتحديث فوري للقلب + عدّاد المفضلة بالسايدبار.
- الأحداث مفوّضة (Event delegation) على الحاوية لأن الشبكة تُستبدل بالـ AJAX.

## بيانات تجريبية (Dev فقط)
`Infrastructure/Data/DevDataSeeder.cs`: يزرع الـ 13 جامعة من التصميم (7 حكومية / 6 خاصة) + 3 مدن + 17 تخصص، بترجمات en/ar/tr (tk/fa تسقط للإنكليزية عبر الـ Fallback). يعمل مرة واحدة فقط وفي بيئة التطوير فقط.

## إصلاح جوهري: الترجمة كانت معطّلة بصمت
**العرض:** المفاتيح الجديدة ظهرت كنص حرفي (`HeroTitle`)، والقديمة "بدت سليمة" فقط لأن قيمتها الإنكليزية تطابق اسم المفتاح.
**السبب:** عدم تطابق اسم المورد المضمّن مع اسم البحث:
- الـ resx يُضمّن باسم يتبع **namespace الصف المجاور** (`SharedResources.cs`) وليس مسار المجلد.
- بينما `ResourcesPath = "Resources"` تجعل الـ Localizer يبحث عن `Lumora.Web.Resources.SharedResources` (ومع namespace الفرعي القديم كان يبحث عن `...Resources.Resources...`).

**الحل:**
1. نقل صف `SharedResources` إلى الـ namespace الجذري `Lumora.Web` (الملف بقي مكانه في مجلد Resources).
2. إزالة `ResourcesPath` من `AddLocalization()` — فيتطابق اسم البحث `Lumora.Web.SharedResources` مع اسم التضمين تماماً.
3. توحيد الحقن عبر `_ViewImports.cshtml`: `@inject IHtmlLocalizer<SharedResources> SharedLocalizer` متاح لكل الـ Views (بديلاً عن `IViewLocalizer` الذي كان يبحث عن resx لكل View على حدة — غير موجودة).

## نتائج الاختبار (فعلياً على السيرفر المحلي)
| الفحص | النتيجة |
|---|---|
| `/Universities` (en) | 200 — 12 بطاقة (صفحة 1 من 13)، العنوان مترجم |
| `?culture=ar&ui-culture=ar` | `dir="rtl"` + Bootstrap RTL + كل النصوص عربية |
| `List?Type=Private` | 6 بطاقات خاصة فقط ✓ (مطابق للتصميم 6/7) |
| `List?Page=2` | بطاقة واحدة (13 = 12+1) ✓ |
| `List?SearchTerm=Medicine` | 8 جامعات ✓ |
| `List?SearchTerm=طب` | 9 جامعات (شمل "طب أسنان") — البحث عابر اللغات ✓ |
| `List?CityId=3` (إزمير) | جامعة واحدة (إيجه) ✓ |
| `List?MaxAnnualTuitionUsd=1000` | 7 جامعات ✓ |
| POST `/Favorites/Toggle` (مجهول) | 302 → Login ✓ |

## المتبقي لمراحل قادمة
- زر "قدّم الآن" وزر "استشارة مجانية" حالياً روابط Placeholder — سيُربطان بنموذج طلب الاستشارة (يتطلب دخول طالب).
- صفحات Services / Housing / About في الـ Navbar روابط Placeholder.
- التحقق البصري النهائي بمتصفح حقيقي (المطلوب من المستخدم: تشغيل `dotnet run` وفتح `/Universities` ومقارنة بالتصميم).
