using Lumora.Domain.Constants;
using Lumora.Domain.Entities;
using Lumora.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Data;

/// Development-only demo data: the 13 universities from the approved design mocks.
/// Runs once — skipped as soon as any university exists. Seeds en/ar/tr translations
/// (tk/fa fall back to English via GetTranslationWithFallback).
public static class DevDataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await SeedUniversitiesAsync(context);
        await SeedHousingsAsync(context);
        await SeedTestimonialsAsync(context);
        await SeedFaqsAsync(context);
    }

    private static async Task SeedUniversitiesAsync(ApplicationDbContext context)
    {
        if (await context.Universities.AnyAsync())
        {
            return;
        }

        var cities = new Dictionary<string, City>
        {
            ["Istanbul"] = NewCity("Istanbul", "إسطنبول", "İstanbul"),
            ["Ankara"]   = NewCity("Ankara", "أنقرة", "Ankara"),
            ["Izmir"]    = NewCity("Izmir", "إزمير", "İzmir")
        };

        var programs = new Dictionary<string, AcademicProgram>
        {
            ["Engineering"]  = NewProgram("Engineering", "هندسة", "Mühendislik"),
            ["Economics"]    = NewProgram("Economics", "اقتصاد", "Ekonomi"),
            ["Business"]     = NewProgram("Business", "إدارة أعمال", "İşletme"),
            ["Medicine"]     = NewProgram("Medicine", "طب", "Tıp"),
            ["Law"]          = NewProgram("Law", "حقوق", "Hukuk"),
            ["Architecture"] = NewProgram("Architecture", "عمارة", "Mimarlık"),
            ["CS"]           = NewProgram("Computer Science", "حاسوب", "Bilgisayar"),
            ["Management"]   = NewProgram("Management", "إدارة", "Yönetim"),
            ["Arts"]         = NewProgram("Arts", "فنون", "Sanat"),
            ["Politics"]     = NewProgram("Political Science", "علوم سياسية", "Siyaset Bilimi"),
            ["Pharmacy"]     = NewProgram("Pharmacy", "صيدلة", "Eczacılık"),
            ["Nursing"]      = NewProgram("Nursing", "تمريض", "Hemşirelik"),
            ["Media"]        = NewProgram("Media", "إعلام", "Medya"),
            ["Agriculture"]  = NewProgram("Agriculture", "زراعة", "Ziraat"),
            ["Science"]      = NewProgram("Science", "علوم", "Fen"),
            ["Education"]    = NewProgram("Education", "تربية", "Eğitim"),
            ["Dentistry"]    = NewProgram("Dentistry", "طب أسنان", "Diş Hekimliği")
        };

        var universities = new[]
        {
            NewUniversity("Boğaziçi University", "جامعة بوغازيتشي", "Boğaziçi Üniversitesi",
                UniversityType.Public, 4.9m, 700, 3, cities["Istanbul"],
                [programs["Engineering"], programs["Economics"], programs["Business"]]),
            NewUniversity("Koç University", "جامعة كوتش", "Koç Üniversitesi",
                UniversityType.Private, 4.9m, 22000, 1, cities["Istanbul"],
                [programs["Medicine"], programs["Engineering"], programs["Business"]]),
            NewUniversity("Bilkent University", "جامعة بيلكنت", "Bilkent Üniversitesi",
                UniversityType.Private, 4.9m, 15000, 1, cities["Ankara"],
                [programs["Engineering"], programs["Architecture"], programs["Law"]]),
            NewUniversity("Istanbul University", "جامعة إسطنبول", "İstanbul Üniversitesi",
                UniversityType.Public, 4.8m, 800, 5, cities["Istanbul"],
                [programs["Medicine"], programs["Engineering"], programs["Law"]]),
            NewUniversity("Istanbul Technical Univ.", "جامعة إسطنبول التقنية", "İstanbul Teknik Üniversitesi",
                UniversityType.Public, 4.8m, 1000, 2, cities["Istanbul"],
                [programs["Engineering"], programs["Architecture"], programs["CS"]]),
            NewUniversity("Sabancı University", "جامعة صبنجة", "Sabancı Üniversitesi",
                UniversityType.Private, 4.8m, 20000, 1, cities["Istanbul"],
                [programs["Engineering"], programs["Management"], programs["Arts"]]),
            NewUniversity("Ankara University", "جامعة أنقرة", "Ankara Üniversitesi",
                UniversityType.Public, 4.7m, 750, 4, cities["Ankara"],
                [programs["Medicine"], programs["Politics"], programs["Pharmacy"]]),
            NewUniversity("Hacettepe University", "جامعة حاجت تبه", "Hacettepe Üniversitesi",
                UniversityType.Public, 4.7m, 900, 3, cities["Ankara"],
                [programs["Medicine"], programs["Nursing"], programs["Engineering"]]),
            NewUniversity("Bahçeşehir University", "جامعة بهتشة شهير", "Bahçeşehir Üniversitesi",
                UniversityType.Private, 4.7m, 6500, 4, cities["Istanbul"],
                [programs["Medicine"], programs["Engineering"], programs["Media"]]),
            NewUniversity("Ege University", "جامعة إيجه", "Ege Üniversitesi",
                UniversityType.Public, 4.6m, 650, 3, cities["Izmir"],
                [programs["Agriculture"], programs["Medicine"], programs["Science"]]),
            NewUniversity("Gazi University", "جامعة غازي", "Gazi Üniversitesi",
                UniversityType.Public, 4.6m, 700, 4, cities["Ankara"],
                [programs["Education"], programs["Engineering"], programs["Medicine"]]),
            NewUniversity("Istanbul Aydın University", "جامعة إسطنبول أيدن", "İstanbul Aydın Üniversitesi",
                UniversityType.Private, 4.6m, 4500, 3, cities["Istanbul"],
                [programs["Dentistry"], programs["Engineering"], programs["Business"]]),
            NewUniversity("Medipol University", "جامعة مديبول", "Medipol Üniversitesi",
                UniversityType.Private, 4.6m, 8000, 3, cities["Istanbul"],
                [programs["Medicine"], programs["Pharmacy"], programs["Dentistry"]])
        };

        context.Cities.AddRange(cities.Values);
        context.AcademicPrograms.AddRange(programs.Values);
        context.Universities.AddRange(universities);
        await context.SaveChangesAsync();
    }

    // Separate guard from universities so it also fills databases seeded before housings existed.
    private static async Task SeedHousingsAsync(ApplicationDbContext context)
    {
        if (await context.Housings.AnyAsync())
        {
            return;
        }

        // Cities were created by SeedUniversitiesAsync (or exist already) — look them up by English name.
        var cities = await context.Cities
            .Include(c => c.Translations)
            .Where(c => !c.IsDeleted)
            .ToListAsync();

        City CityByName(string en) =>
            cities.First(c => c.Translations.Any(t => t.LanguageCode == LanguageConstants.English && t.Name == en));

        var istanbul = CityByName("Istanbul");
        var ankara = CityByName("Ankara");
        var izmir = CityByName("Izmir");

        var housings = new[]
        {
            NewHousing(istanbul, 320,
                "Beyazıt Student Dorm", "Modern dorm 5 minutes from Istanbul University. Single and double rooms with study areas, laundry and 24/7 security.", "Beyazıt, Fatih, Istanbul",
                "سكن بيازيت الطلابي", "سكن حديث على بعد 5 دقائق من جامعة إسطنبول. غرف مفردة ومزدوجة مع قاعات دراسة ومغسلة وحراسة على مدار الساعة.", "بيازيت، الفاتح، إسطنبول",
                "Beyazıt Öğrenci Yurdu", "İstanbul Üniversitesi'ne 5 dakika mesafede modern yurt. Çalışma alanları, çamaşırhane ve 7/24 güvenlikli tek ve çift kişilik odalar.", "Beyazıt, Fatih, İstanbul"),
            NewHousing(istanbul, 450,
                "Beşiktaş Shared Apartments", "Furnished shared apartments near Boğaziçi and Bahçeşehir universities. Kitchen, fast internet and bills included.", "Beşiktaş, Istanbul",
                "شقق بشكتاش المشتركة", "شقق مفروشة مشتركة قرب جامعتي بوغازيتشي وبهتشة شهير. مطبخ وإنترنت سريع والفواتير مشمولة.", "بشكتاش، إسطنبول",
                "Beşiktaş Paylaşımlı Daireler", "Boğaziçi ve Bahçeşehir üniversitelerine yakın mobilyalı paylaşımlı daireler. Mutfak, hızlı internet ve faturalar dahil.", "Beşiktaş, İstanbul"),
            NewHousing(istanbul, 600,
                "Üsküdar Private Studio", "Private studio flats on the Asian side with metro access to Medipol and Aydın campuses. Ideal for students who prefer quiet.", "Üsküdar, Istanbul",
                "ستوديو أسكودار الخاص", "شقق ستوديو خاصة في الجانب الآسيوي مع وصول مترو لحرمي مديبول وأيدن. مثالية لمن يفضل الهدوء.", "أسكودار، إسطنبول",
                "Üsküdar Özel Stüdyo", "Anadolu yakasında, Medipol ve Aydın kampüslerine metro erişimli özel stüdyo daireler. Sessizlik tercih eden öğrenciler için ideal.", "Üsküdar, İstanbul"),
            NewHousing(ankara, 250,
                "Çankaya Student Residence", "Budget-friendly residence near Ankara and Hacettepe universities. Shared kitchens, gym and study lounges.", "Çankaya, Ankara",
                "سكن تشانكايا الطلابي", "سكن اقتصادي قرب جامعتي أنقرة وحاجت تبه. مطابخ مشتركة وصالة رياضة وقاعات دراسة.", "تشانكايا، أنقرة",
                "Çankaya Öğrenci Rezidansı", "Ankara ve Hacettepe üniversitelerine yakın uygun fiyatlı rezidans. Ortak mutfaklar, spor salonu ve çalışma salonları.", "Çankaya, Ankara"),
            NewHousing(ankara, 380,
                "Bilkent Campus Apartments", "On-campus style apartments next to Bilkent University with shuttle service and cafeteria plans.", "Bilkent, Ankara",
                "شقق حرم بيلكنت", "شقق بنمط الحرم الجامعي بجوار جامعة بيلكنت مع خدمة نقل وخطط وجبات.", "بيلكنت، أنقرة",
                "Bilkent Kampüs Daireleri", "Bilkent Üniversitesi'nin yanında kampüs tarzı daireler; servis ve yemekhane planlarıyla.", "Bilkent, Ankara"),
            NewHousing(izmir, 280,
                "Bornova Student House", "Friendly student house minutes from Ege University. Garden, common areas and weekly cleaning included.", "Bornova, Izmir",
                "بيت بورنوفا الطلابي", "سكن طلابي ودود على بعد دقائق من جامعة إيجه. حديقة ومساحات مشتركة وتنظيف أسبوعي مشمول.", "بورنوفا، إزمير",
                "Bornova Öğrenci Evi", "Ege Üniversitesi'ne dakikalar uzaklıkta samimi öğrenci evi. Bahçe, ortak alanlar ve haftalık temizlik dahil.", "Bornova, İzmir")
        };

        context.Housings.AddRange(housings);
        await context.SaveChangesAsync();
    }

    private static Housing NewHousing(
        City city, decimal pricePerMonth,
        string enTitle, string enDesc, string enAddr,
        string arTitle, string arDesc, string arAddr,
        string trTitle, string trDesc, string trAddr) => new()
    {
        City = city,
        PricePerMonthUsd = pricePerMonth,
        Translations =
        [
            new HousingTranslation { LanguageCode = LanguageConstants.English, Title = enTitle, Description = enDesc, Address = enAddr },
            new HousingTranslation { LanguageCode = LanguageConstants.Arabic, Title = arTitle, Description = arDesc, Address = arAddr },
            new HousingTranslation { LanguageCode = LanguageConstants.Turkish, Title = trTitle, Description = trDesc, Address = trAddr }
        ]
    };

    private static async Task SeedTestimonialsAsync(ApplicationDbContext context)
    {
        if (await context.Testimonials.AnyAsync())
        {
            return;
        }

        var testimonials = new[]
        {
            NewTestimonial(1,
                "Ahmed Al-Sayed", "Computer Engineering Student, Istanbul Technical University", "Lumora handled everything — from choosing the program to my acceptance letter. I only had to pack my bags.",
                "أحمد السيد", "طالب هندسة حاسوب، جامعة إسطنبول التقنية", "لومورا تولت كل شيء — من اختيار التخصص حتى قبولي الجامعي. لم يتبق لي سوى حزم حقائبي.",
                "Ahmed Al-Sayed", "Bilgisayar Mühendisliği Öğrencisi, İstanbul Teknik Üniversitesi", "Lumora her şeyi halletti — bölüm seçiminden kabul mektubuma kadar. Bana sadece valiz hazırlamak kaldı."),
            NewTestimonial(2,
                "Leyla Rahimi", "Medicine Student, Ankara University", "The free consultation saved me months of research. They knew exactly which universities fit my grades and budget.",
                "ليلى رحيمي", "طالبة طب، جامعة أنقرة", "الاستشارة المجانية وفرت عليّ شهوراً من البحث. عرفوا بالضبط أي الجامعات تناسب معدلي وميزانيتي.",
                "Leyla Rahimi", "Tıp Öğrencisi, Ankara Üniversitesi", "Ücretsiz danışmanlık bana aylarca sürecek araştırmayı kazandırdı. Notlarıma ve bütçeme hangi üniversitelerin uyduğunu tam olarak biliyorlardı."),
            NewTestimonial(3,
                "Yusuf Kerimov", "Business Student, Bahçeşehir University", "They found me housing five minutes from campus before I even landed in Istanbul. Real support, not just promises.",
                "يوسف كريموف", "طالب إدارة أعمال، جامعة بهتشة شهير", "وجدوا لي سكناً على بعد خمس دقائق من الحرم قبل أن أهبط في إسطنبول أصلاً. دعم حقيقي وليس مجرد وعود.",
                "Yusuf Kerimov", "İşletme Öğrencisi, Bahçeşehir Üniversitesi", "Ben daha İstanbul'a inmeden kampüse beş dakika mesafede konaklama buldular. Sadece söz değil, gerçek destek.")
        };

        context.Testimonials.AddRange(testimonials);
        await context.SaveChangesAsync();
    }

    private static Testimonial NewTestimonial(
        int order,
        string enName, string enTitle, string enQuote,
        string arName, string arTitle, string arQuote,
        string trName, string trTitle, string trQuote) => new()
    {
        DisplayOrder = order,
        IsPublished = true,
        Translations =
        [
            new TestimonialTranslation { LanguageCode = LanguageConstants.English, StudentName = enName, StudentTitle = enTitle, QuoteText = enQuote },
            new TestimonialTranslation { LanguageCode = LanguageConstants.Arabic, StudentName = arName, StudentTitle = arTitle, QuoteText = arQuote },
            new TestimonialTranslation { LanguageCode = LanguageConstants.Turkish, StudentName = trName, StudentTitle = trTitle, QuoteText = trQuote }
        ]
    };

    private static async Task SeedFaqsAsync(ApplicationDbContext context)
    {
        if (await context.Faqs.AnyAsync())
        {
            return;
        }

        var faqs = new[]
        {
            NewFaq(1,
                "How do I apply to a Turkish university through Lumora?", "Create a student account, browse the directory, and press \"Apply now\" on the university you like — or request a free consultation and our advisors will shortlist universities for you. We prepare and submit the application on your behalf.",
                "كيف أقدّم على جامعة تركية عبر لومورا؟", "أنشئ حساب طالب، تصفح الدليل، واضغط \"قدّم الآن\" على الجامعة التي تعجبك — أو اطلب استشارة مجانية وسيرشح مستشارونا جامعات تناسبك. نحن نجهز الطلب ونقدمه نيابة عنك.",
                "Lumora üzerinden bir Türk üniversitesine nasıl başvururum?", "Öğrenci hesabı oluşturun, rehberi inceleyin ve beğendiğiniz üniversitede \"Şimdi başvur\"a basın — veya ücretsiz danışmanlık isteyin, danışmanlarımız size uygun üniversiteleri listelesin. Başvuruyu sizin adınıza hazırlayıp gönderiyoruz."),
            NewFaq(2,
                "How much does studying in Türkiye cost?", "Public universities charge roughly $500–$1,000 per year, while private universities range from about $4,000 to $22,000. Living costs are typically $300–$600 per month depending on the city.",
                "كم تكلف الدراسة في تركيا؟", "تتراوح رسوم الجامعات الحكومية تقريباً بين 500 و1000 دولار سنوياً، بينما تتراوح رسوم الجامعات الخاصة بين 4000 و22000 دولار. وتكاليف المعيشة عادة بين 300 و600 دولار شهرياً حسب المدينة.",
                "Türkiye'de okumak ne kadara mal olur?", "Devlet üniversiteleri yılda yaklaşık 500–1.000 dolar alırken, özel üniversiteler yaklaşık 4.000 ile 22.000 dolar arasında değişir. Yaşam maliyeti şehre bağlı olarak genellikle ayda 300–600 dolardır."),
            NewFaq(3,
                "Do I need to speak Turkish to study in Türkiye?", "Not necessarily. Many programs are taught fully in English. If you choose a Turkish-taught program, universities offer a one-year Turkish preparation (TÖMER) course.",
                "هل أحتاج إلى إتقان التركية للدراسة في تركيا؟", "ليس بالضرورة. كثير من التخصصات تُدرَّس بالإنكليزية بالكامل. وإذا اخترت تخصصاً باللغة التركية، توفر الجامعات سنة تحضيرية للغة (تومر).",
                "Türkiye'de okumak için Türkçe bilmem gerekir mi?", "Şart değil. Birçok bölüm tamamen İngilizce okutulur. Türkçe bir bölüm seçerseniz üniversiteler bir yıllık Türkçe hazırlık (TÖMER) kursu sunar."),
            NewFaq(4,
                "Can Lumora help me find housing?", "Yes — browse the Housing section for dorms and apartments near your campus, or tell your advisor your budget and we will arrange it before you arrive.",
                "هل تساعدني لومورا في إيجاد سكن؟", "نعم — تصفح قسم السكن للاطلاع على السكنات والشقق القريبة من حرمك الجامعي، أو أخبر مستشارك بميزانيتك وسنرتبه لك قبل وصولك.",
                "Lumora konaklama bulmama yardımcı olabilir mi?", "Evet — kampüsünüze yakın yurt ve daireler için Konaklama bölümüne göz atın veya danışmanınıza bütçenizi söyleyin, siz gelmeden ayarlayalım.")
        };

        context.Faqs.AddRange(faqs);
        await context.SaveChangesAsync();
    }

    private static Faq NewFaq(
        int order,
        string enQ, string enA,
        string arQ, string arA,
        string trQ, string trA) => new()
    {
        DisplayOrder = order,
        Translations =
        [
            new FaqTranslation { LanguageCode = LanguageConstants.English, Question = enQ, Answer = enA },
            new FaqTranslation { LanguageCode = LanguageConstants.Arabic, Question = arQ, Answer = arA },
            new FaqTranslation { LanguageCode = LanguageConstants.Turkish, Question = trQ, Answer = trA }
        ]
    };

    private static City NewCity(string en, string ar, string tr) => new()
    {
        Translations =
        [
            new CityTranslation { LanguageCode = LanguageConstants.English, Name = en },
            new CityTranslation { LanguageCode = LanguageConstants.Arabic, Name = ar },
            new CityTranslation { LanguageCode = LanguageConstants.Turkish, Name = tr }
        ]
    };

    private static AcademicProgram NewProgram(string en, string ar, string tr) => new()
    {
        Translations =
        [
            new AcademicProgramTranslation { LanguageCode = LanguageConstants.English, Name = en },
            new AcademicProgramTranslation { LanguageCode = LanguageConstants.Arabic, Name = ar },
            new AcademicProgramTranslation { LanguageCode = LanguageConstants.Turkish, Name = tr }
        ]
    };

    private static University NewUniversity(
        string en, string ar, string tr,
        UniversityType type, decimal rating, decimal tuition, int campuses,
        City city, AcademicProgram[] programs) => new()
    {
        Type = type,
        Rating = rating,
        AnnualTuitionUsd = tuition,
        CampusCount = campuses,
        City = city,
        Programs = programs,
        Translations =
        [
            new UniversityTranslation { LanguageCode = LanguageConstants.English, Name = en, ShortDescription = en },
            new UniversityTranslation { LanguageCode = LanguageConstants.Arabic, Name = ar, ShortDescription = ar },
            new UniversityTranslation { LanguageCode = LanguageConstants.Turkish, Name = tr, ShortDescription = tr }
        ]
    };
}
