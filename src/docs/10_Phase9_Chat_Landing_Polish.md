# Phase 9 — الشات المباشر + الصفحة الرئيسية + التلميع الشامل

> أُنجز على جلستين متتاليتين (مع جلسة 07 للكماليات و08/09 للسكن والصفحات الثابتة). هذا الملف يغطي: نظام المحادثات SignalR، الصفحة الرئيسية من التصميم، الفوتر الجديد + الشريط العائم، إعادة تصميم صفحات الحساب والاستشارة، وتنظيم الداشبورد.

## 1. الشات المباشر (طالب ↔ فريق الدعم)

### قاعدة البيانات (Migration: `AddChat`)
- `ChatConversations`: محادثة واحدة لكل طالب (فهرس فريد `StudentUserId`)، `LastMessageAt` denormalized لترتيب الـ Inbox. FK لـ AspNetUsers بـ Restrict.
- `ChatMessages`: `ConversationId` (Cascade)، `SenderUserId`، `IsFromAdmin`، `Text` (2000)، `IsRead`، فهرس `(ConversationId, CreatedAt)`.

### الطبقات
- `IChatService`/`ChatService` (Application): GetOrCreate، GetMessages، Send، GetConversationsForAdmin (مع Preview وعداد غير المقروء)، MarkRead(readerIsAdmin) — "القارئ يستهلك رسائل الطرف الآخر".
- `Hubs/ChatHub.cs` (Web): `[Authorize]`؛ مجموعة لكل محادثة `conv-{id}`. الطالب `JoinMyConversation()` (لا يستطيع دخول غير محادثته)، الأدمن `JoinConversation(id)`. `SendMessage` يتحقق من الملكية ثم يحفظ **ثم** يبث — ما تراه هو ما انحفظ.
- `signalr.min.js` 8.0.7 منزّلة محلياً في `wwwroot/lib/signalr/` (بلا CDN).
- `Program.cs`: `AddSignalR()` + `MapHub<ChatHub>("/hubs/chat")`.

### الواجهات
- الطالب: `/Chat` (`ChatController` + `Views/Chat/Index.cshtml`).
- الأدمن: `Admin/Chat` — Inbox (اسم الطالب من UserManager، Preview، بادج غير المقروء، ترتيب بالأحدث) + `Conversation/{id}`.
- ملفات مشتركة: `css/pages/chat.css` + `js/pages/chat.js` (نفس السكربت للطرفين — `data-viewer` يحدد أي فقاعة "لي"). دخل قسم Styles إلى `_AdminLayout` لأجل هذا.

### الاختبار (E2E حقيقي عبر Long-Polling بسكربت Python)
negotiate ✓ → handshake ✓ → `JoinMyConversation` أعادت `conversationId=1` ✓ → `SendMessage("Hello from E2E test — مرحبا")` ✓ → **البث رجع بنفس النص** ✓ → صندوق الأدمن أظهر الـ Preview بالعربي + بادج "1 غير مقروء" ✓ → شاشة المحادثة أظهرت فقاعة الطالب ✓. (السكربت محفوظ بالـ scratchpad كمرجع: تفاوض ثم POST handshake `{"protocol":"json","version":1}\x1e` ثم Invocations بنفس الـ URL مع Poll GET بثريد منفصل.)

## 2. الصفحة الرئيسية (من الـ PDF المعتمد)
`HomeController` (أعيدت كتابته): أعلى 6 جامعات (TopRated) + آراء منشورة (3) + مفضلة الطالب الحالي.
أقسام `Views/Home/Index.cshtml` + `css/pages/home.css`: Hero فاتح بتوهج ذهبي + شارة + زرّي CTA + **إحصائيات (5000+/120+/10+/98%)** — خدماتنا (4 بطاقات) — أبرز الجامعات (بطاقات المكوّن المشترك + "عرض الكل") — بلوك السكن — **"لماذا لومورا؟"** (شريط كحلي بـ 4 ميزات) — **"كيف نعمل"** (5 خطوات مرقمة ذهبية) — آراء الطلاب — شريط CTA أخير.

## 3. الفوتر + الشريط العائم + التوپبار
- فوتر 4 أعمدة (Grid متجاوب): البراند + السوشال (واتساب/إنستغرام/فيسبوك بأيقونات SVG أصلية وهوفر بألوان المنصات) | روابط سريعة | خدماتنا | تواصل (موقع/هاتف/بريد).
- **الفوتر ثابت بالأسفل** (Sticky): `body:not(.admin-body)` عمود Flex والـ main يتمدد.
- **الشريط العائم الجانبي** (`.lum-float-dock` بالـ Layout العام): واتساب أخضر + محادثة كحلي + مفضلة وردي، مع Tooltip من `data-tip` — الزائر غير المسجل يضغط محادثة/مفضلة فيتحول لتسجيل الدخول تلقائياً.
- **رقم الواتساب الحقيقي للمالك** (905359330591) موحّد: الزر العائم + سوشال الفوتر + هاتف الفوتر والتوپبار. البريد وحسابات إنستا/فيسبوك ما زالت Placeholders.

## 4. صفحات الحساب (Login/Register/Profile/ChangePassword)
Shell مقسوم: **لوحة براند كحلية** (لوغو LUM●RA بنقطة ذهبية نابضة + Tagline + 3 مزايا بظهور متدرج Staggered) + لوحة الفورم. مشترك عبر `Views/Shared/_AuthBrandPanel.cshtml`.
- `css/pages/auth.css`: أنيميشن دخول، توهج يتبع الماوس، فوكس ذهبي للحقول، زر Submit بضغطة مرنة.
- `js/pages/auth.js`: **زر إظهار/إخفاء كلمة المرور** يُحقن تلقائياً بكل حقل password، الـ Parallax للتوهج، وتفعيل الأنيميشن.

## 5. صفحة طلب الاستشارة
نفس الـ Shell مع لوحة براند فيها عنوان "مستعد تبدأ رحلتك؟" + مزايا + **نقاط تقدم 1-2-3**، وحقل الرسالة مع **عدّاد حروف حي** (`js/pages/consultation.js`) وChip باسم الجامعة عند القدوم من "قدّم الآن".

## 6. داشبورد الأدمن المنظم
صفّان بعناوين: **النشاط** (استشارات جديدة تتلون أحمر، الرسائل، الطلاب) و**المحتوى** (جامعات/سكن/مدن/تخصصات/أسئلة/آراء). كل بطاقة بأيقونة دائرية ذهبية + هوفر يرفعها + **الأرقام تعد تصاعدياً** عند فتح الصفحة (`data-count` بـ admin.js).

## نتائج الفحص النهائي
كل الصفحات العامة الثمانية 200 (الرئيسية، الجامعات، السكن، الخدمات، من نحن، الأسئلة، دخول، تسجيل) — الدوك العائم 3 أزرار — رقم المالك بـ 4 مواضع — الداشبورد 9 أيقونات و8 عدادات وقسمان — شات E2E كامل (أعلاه).

## Placeholders متبقية للمالك
روابط إنستغرام/فيسبوك (`instagram.com/lumora`, `facebook.com/lumora`) والبريد `info@lumora.com` بالفوتر والتوپبار — عند توفر الحسابات الحقيقية تُحدّث في `Footer/Default.cshtml` + `Navbar/Default.cshtml` + الزر العائم في `_Layout.cshtml`.
