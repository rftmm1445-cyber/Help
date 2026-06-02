using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication4.GameModels
{
    public class Hint
    {
        // 1. الخصائص الأساسية المعتمدة في مشروعك
        public string hintType { get; set; }
        public double hintlong { get; set; }
        public double hintlat { get; set; }

        // ربط التسميات ليتوافق الكود الداخلي مع الخصائص الممررة
        public double Latitude
        {
            get => hintlat;
            set => hintlat = value;
        }

        public double Longitude
        {
            get => hintlong;
            set => hintlong = value;
        }

        // الخاصية التي ستحمل نص التلميح الغامض المرتجع من الذكاء الاصطناعي
        public string GeneratedHint { get; set; }


        // 2. كائن الـ HttpClient المستقر لإعادة الاستخدام ومنع الضغط على السيرفر
        private static readonly HttpClient _client = new HttpClient();


        // 3. الدالة الذكية والمحمية (تعتمد على الإحداثيات فقط دون تمرير اسم الدولة)
        public async Task FetchAndSetHintFromAiAsync(string apiKey)
        {
            try
            {
                // رابط الاتصال بموديل Gemini 3.5 Flash السريع
                string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.5-flash:generateContent?key={apiKey}";

                // صياغة البرومبت الذكي: نطلب من الموديل أولاً تحديد الدولة بنفسه من الإحداثيات مخفياً، ثم صياغة الفزورة
                string prompt = $"أنا أعطيك إحداثيات جغرافية دقيقة وهي:\n" +
                                $"- خط العرض: {this.Latitude}\n" +
                                $"- خط الطول: {this.Longitude}\n\n" +
                                $"المطلوب منك خلف الكواليس معرفة الدولة والمعلم الموجود في هذه الإحداثيات، ثم صياغة تلميح بمستوى متوسط الصعوبة يعتمد على (الصفة والميزة الفريدة) في فئة \"{this.hintType}\" لهذا المكان.\n\n" +
                                $"شروط صارمة:\n" +
                                $"1. اكتب التلميح بأسلوب فزورة ذكية وغامضة ومثيرة للتحدي الثقافي الجغرافي.\n" +
                                $"2. لا تذكر اسم المعلم أو اسم المدينة أو اسم الدولة بشكل مباشر أبداً في النص لكي لا يغش اللاعب الإجابة.";

                // تجهيز جسم الطلب بصيغة الهيكل المتوافق مع سيرفرات جوجل
                var requestBody = new
                {
                    contents = new[]
                    {
                        new { parts = new[] { new { text = prompt } } }
                    }
                };

                // تحويل الكائن إلى JSON نقي
                string jsonPayload = System.Text.Json.JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // إرسال الطلب (POST Request)
                var response = await _client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();

                    // تحليل الـ JSON واستخراج نص الفزورة المرتجع من جوجل
                    using (var doc = System.Text.Json.JsonDocument.Parse(responseString))
                    {
                        string aiText = doc.RootElement
                            .GetProperty("candidates")[0]
                            .GetProperty("content")
                            .GetProperty("parts")[0]
                            .GetProperty("text").GetString();

                        // تنظيف النص وتخزينه مباشرة داخل خاصية GeneratedHint ليكون جاهزاً للعرض
                        this.GeneratedHint = aiText.Trim();
                    }
                }
                else
                {
                    this.GeneratedHint = $"⚠️ خطأ في الاتصال بالذكاء الاصطناعي: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                this.GeneratedHint = $"❌ فشل توليد التلميح الآمن بسبب: {ex.Message}";
            }
        }
    }
}