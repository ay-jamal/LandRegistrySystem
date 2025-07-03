using LandRegistrySystem_Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using System.Text;

namespace LandRegistrySystem_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class PrinterController : ControllerBase
    {


        [HttpPost("print-text")]
        public IActionResult PrintText([FromBody] PrintTextRequest request)
        {
            try
            {
                string printerName = "Xprinter XP-237B"; // تأكد أنه مطابق في ويندوز
                string text = request.Text + "\n\n";

                // طباعة النص كما هو للطابعة
                bool printed = RawPrinterHelper.SendStringToPrinter(printerName, text);

                if (printed)
                    return Ok(new { message = "تمت الطباعة بنجاح." });
                else
                    return StatusCode(500, "فشل في إرسال الأمر للطابعة.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطأ أثناء الطباعة: {ex.Message}");
            }
        }

        [HttpPost("feed-paper")]
        public IActionResult FeedPaper()
        {
            try
            {
                string printerName = "Xprinter XP-237B"; // اسم الطابعة بالضبط كما في ويندوز

                // أمر تغذية 3 أسطر (يمكنك تغيير العدد من 1 إلى 255)
                byte[] feedCmd = new byte[] { 0x1B, 0x64, 0x03 }; // ESC d n   ← n = عدد الأسطر

                // إرسال الأمر
                IntPtr unmanagedPointer = Marshal.AllocHGlobal(feedCmd.Length);
                Marshal.Copy(feedCmd, 0, unmanagedPointer, feedCmd.Length);
                bool printed = RawPrinterHelper.SendBytesToPrinter(printerName, unmanagedPointer, feedCmd.Length);
                Marshal.FreeHGlobal(unmanagedPointer);

                if (printed)
                    return Ok(new { message = "تم تغذية الورق (Feed) بنجاح." });
                else
                    return StatusCode(500, "فشل في إرسال أمر التغذية للطابعة.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطأ أثناء الطباعة: {ex.Message}");
            }
        }



        public class PrintTextRequest
        {
            public string Text { get; set; }
        }
    }
}
