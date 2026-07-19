# Phase 8 (جزء 1) — صفحات السكن العامة

## الهدف
صفحة سكنات عامة بنفس روح صفحة الجامعات (Hero + فلاتر جانبية + بطاقات + AJAX) مع صفحة تفاصيل لكل سكن — البيانات تُدار بالكامل من لوحة الأدمن (Admin → Housing) الجاهزة من Phase 6.

## الملفات

### Controller & ViewModel
| الملف | الوصف |
|---|---|
| `Controllers/HousingsController.cs` (عام) | `Index` (الصفحة الكاملة، الفلاتر من الـ QueryString فالروابط قابلة للمشاركة) + `List` (جزء AJAX) + `Details(id)` (404 لغير الموجود). لا يوجد تعارض مع `Areas/Admin` HousingsController — Areas تفصل بينهما. |
| `Models/Housings/HousingPageViewModels.cs` | `HousingIndexViewModel` (Filter + Cities + Results) |

### Views
| الملف | الوصف |
|---|---|
| `Views/Housings/Index.cshtml` | Hero + سايدبار + شبكة |
| `Views/Housings/_FiltersSidebar.cshtml` | رقائق المدن + سلايدر السعر الأقصى ($100–$2,000+) + Reset |
| `Views/Housings/_HousingList.cshtml` | الجزء المستبدل بالـ AJAX: شبكة + ترقيم + حالة فارغة، يحمل `data-total` |
| `Views/Housings/_HousingCard.cshtml` | بطاقة: صورة/أيقونة منزل + **شارة السعر الذهبية** على الصورة + العنوان (رابط للتفاصيل) + المدينة/العنوان + وصف بسطرين (line-clamp) + زر "عرض التفاصيل" |
| `Views/Housings/Details.cshtml` | Hero بالصورة والعنوان والمدينة، كرت "عن هذا السكن"، كرت "العنوان"، وكرت CTA لاصق بالسعر الشهري + زر "استشارة مجانية" |

### Front-End المعزول (القاعدة نفسها: صفر Inline)
- `wwwroot/css/pages/housings.css` و `housing-details.css` — Logical Properties حصراً.
- `wwwroot/js/pages/housings.js` — رقائق المدن (اختيار مفرد قابل للإلغاء)، سلايدر السعر مع Debounce، ترقيم مفوَّض، تحديث الرابط بـ `replaceState`، حارس تسلسل ضد الاستجابات المتأخرة. (يغطيهما glob تصغير WebOptimizer الموجود.)

### أخرى
- **روابط الـ Navbar والـ Footer**: "السكن" صار يشير لـ `/Housings` مع حالة Active.
- **مفاتيح ترجمة** (9 × 5 لغات): HousingHeroTitle/Subtitle، MaxPrice، PerMonth، NoHousingResults، ViewDetails، AboutHousing، AddressLabel، MatchingHousing.
- **بيانات تجريبية**: `DevDataSeeder` أُعيدت هيكلته لدالتين بحارسين مستقلين (`SeedUniversitiesAsync` / `SeedHousingsAsync`) — سكنات الـ demo الست تنزرع حتى على قاعدة قديمة سبق أن زُرعت جامعاتها. 6 سكنات (3 إسطنبول، 2 أنقرة، 1 إزمير) بأسعار $250–$600 وترجمات en/ar/tr.

## نتائج الاختبار الفعلي
| الفحص | النتيجة |
|---|---|
| `/Housings` | 200 — 6 بطاقات والعداد 6 |
| `List?CityId=1` (إسطنبول) | 3 ✓ |
| `List?CityId=2` (أنقرة) | 2 ✓ |
| `List?MaxPricePerMonthUsd=300` | 2 ✓ (250 و280) |
| فلتر مدمج إسطنبول + ≤350 | 1 ✓ (320 فقط) |
| `/Housings/Details/1` | 200 بعنوان "Beyazıt Student Dorm" |
| `/Housings/Details/999` | 404 ✓ |
| العربية | `dir="rtl"` + "اعثر على سكن طلابي مريح" + أسماء السكنات بالعربية ✓ |
| حالة Active بالـ Navbar | ✓ |

## مصيدة تكررت (تذكير)
البناء فشل أول مرة بـ MSB3021/MSB3027 لأن سيرفر اختبار قديم كان ماسك الـ DLLs — **اقتل `Lumora.Web` قبل البناء** لا بعده (المصيدة رقم 8 في START_HERE).

## المتبقي من Phase 8
صفحات Services و About (+آراء الطلاب من جدول Testimonials) و FAQ العامة — البيانات والخدمات جاهزة، ينقص Controllers + Views فقط. بعدها Phase 9: الشات المباشر (التواصل بين الإدارة ومرسلي الاستشارات).
