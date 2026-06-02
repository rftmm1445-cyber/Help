//namespace WebApplication4.GameModels
//{
//    public class Location
//    {
//        public int LocationId { get; set; }              // معرف الموقع
//        public string Namee { get; set; } = string.Empty;  // اسم الموقع
//        public double Latitude { get; set; }            // خط العرض
//        public double Longitude { get; set; }           // خط الطول
//        public int PlayersId { get; set; }              // معرف اللاعب المرتبط بالموقع

//        public Location() { }

//        public Location(int locationId, string namee, double latitude, double longitude, int playersId)
//        {
//            LocationId = locationId;
//            Namee = namee;
//            Latitude = latitude;
//            Longitude = longitude;
//            PlayersId = playersId;
//        }

//    }
//}

using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication4.GameModels
{
    public class Location
    {
        public int LocationId { get; set; }              // معرف الموقع
        public string Namee { get; set; } = string.Empty;  // اسم الموقع
        public double Latitude { get; set; }            // خط العرض
        public double Longitude { get; set; }           // خط الطول
        public int PlayersId { get; set; }              // معرف اللاعب المرتبط بالموقع

        public Location() { }

        public Location(int locationId, string namee, double latitude, double longitude, int playersId)
        {
            LocationId = locationId;
            Namee = namee;
            Latitude = latitude;
            Longitude = longitude;
            PlayersId = playersId;
        }

        // ========================================================
        // الدالة الذكية: تتصل بـ Gemini وتجلب اسم الدولة وتخزنه في Namee
        // ========================================================
        public async Task FetchAndSetNameeFromAiAsync(string apiKey)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // رابط الاتصال المباشر بموديل Gemini 3.5 Flash
                    string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.5-flash:generateContent?key={apiKey}";

                    // صياغة البرومبت الصارم ليعيد كلمة واحدة فقط
                    string prompt = $" أنا أعطيك إحداثيات جغرافية، والمطلوب منك إعطائي اسم الدولة التي تقع فيها هذه الإحداثيات فقط.اي ان اجابتك تكون اسم الدولة فقط بدون اي جمل اضافية " +
                                   $"خط العرض: {this.Latitude} ، خط الطول: {this.Longitude}. " +
                                   $"شروط صارمة: أجب بكلمة واحدة فقط وهي اسم الدولة باللغة العربية دون أي كلمات إضافية أو علامات ترقيم.";

                    // تجهيز جسم الطلب بصيغة JSON التي تفهمها سيرفرات جوجل
                    var requestBody = new
                    {
                        contents = new[]
                        {
                            new { parts = new[] { new { text = prompt } } }
                        }
                    };

                    string jsonPayload = System.Text.Json.JsonSerializer.Serialize(requestBody);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    // إرسال الطلب إلى جوجل
                    var response = await client.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseString = await response.Content.ReadAsStringAsync();

                        // تحليل كود الـ JSON القادم من جوجل لاستخراج الكلمة المطلوبة
                        using (var doc = System.Text.Json.JsonDocument.Parse(responseString))
                        {
                            string aiText = doc.RootElement
                                .GetProperty("candidates")[0]
                                .GetProperty("content")
                                .GetProperty("parts")[0]
                                .GetProperty("text").GetString();

                            // تنظيف النص وتخزينه في خاصية Namee ليكون جاهزاً للحفظ في قاعدة البيانات
                            this.Namee = aiText.Trim();
                        }
                    }
                    else
                    {
                        this.Namee = "خطأ في الاتصال";
                    }
                }
            }
            catch
            {
                this.Namee = "فشل تحديد الدولة";
            }
        }
    
    // ========================================================
        // العملية الثانية والجديدة: جلب معلومة عشوائية غريبة عن الدولة
        // ========================================================
        public async Task<string> GetRandomCountryFactAsync(string apiKey)
        {
            // التحقق أولاً من أن اسم الدولة موجود وليس فارغاً
            if (string.IsNullOrEmpty(this.Namee) || this.Namee == "جاري التحديد..." || this.Namee == "فشل تحديد الدولة")
            {
                return "لا يمكن جلب معلومة لأن اسم الدولة غير محدد بشكل صحيح.";
            }

            try
            {
                using (var client = new HttpClient())
                {
                    string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.5-flash:generateContent?key={apiKey}";

                    // صياغة البرومبت للاعتماد على الاسم الحالي المخزن في الكلاس (this.Namee)
                    string prompt = $"أعطني معلومة عشوائية، غريبة، أو مشوقة لا يعرفها الكثيرون عن دولة ({this.Namee}). " +
                                   $"شروط الإجابة: اكتب المعلومة باللغة العربية بأسلوب ممتع ومختصر جداً (في سطرين أو ثلاثة أسطر كحد أقصى) وبدون مقدمات.";

                    var requestBody = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };
                    string jsonPayload = System.Text.Json.JsonSerializer.Serialize(requestBody);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseString = await response.Content.ReadAsStringAsync();
                        using (var doc = System.Text.Json.JsonDocument.Parse(responseString))
                        {
                            string factText = doc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();
                            return factText.Trim();
                        }
                    }
                    return "تعذر جلب معلومة في الوقت الحالي بسبب مشكلة في الاتصال.";
                }
            }
            catch
            {
                return "حدث خطأ أثناء محاولة جلب المعلومة العشوائية.";
            }
        }
    }
}

