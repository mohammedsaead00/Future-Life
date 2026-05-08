using SendGrid;
using SendGrid.Helpers.Mail;
using FutureLife.API.Models;

namespace FutureLife.API.Services;

public class EmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task<bool> SendMonthlySummaryAsync(User user, SimulationResult? latestResult)
    {
        var apiKey     = _config["SendGrid:ApiKey"];
        var fromEmail  = _config["SendGrid:FromEmail"] ?? "noreply@futurelife.app";
        var fromName   = _config["SendGrid:FromName"]  ?? "FutureLife";

        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogWarning("SendGrid API Key not configured. Skipping email for {Email}", user.Email);
            return false;
        }

        var client  = new SendGridClient(apiKey);
        var from    = new EmailAddress(fromEmail, fromName);
        var to      = new EmailAddress(user.Email, user.FullName);
        var subject = $"📊 تقريرك الشهري — {DateTime.UtcNow:MMMM yyyy} | FutureLife";
        var html    = BuildHtmlEmail(user, latestResult);

        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: null, htmlContent: html);

        try
        {
            var response = await client.SendEmailAsync(msg);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
            {
                _logger.LogInformation("Monthly email sent to {Email}", user.Email);
                return true;
            }
            _logger.LogWarning("SendGrid returned {Status} for {Email}", response.StatusCode, user.Email);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", user.Email);
            return false;
        }
    }

    private static string BuildHtmlEmail(User user, SimulationResult? r)
    {
        // Risk label helpers
        string RiskBadge(double risk) => risk switch
        {
            < 0.3  => "<span style='color:#22c55e;font-weight:bold;'>🟢 منخفض</span>",
            < 0.65 => "<span style='color:#f59e0b;font-weight:bold;'>🟡 متوسط</span>",
            _      => "<span style='color:#ef4444;font-weight:bold;'>🔴 مرتفع</span>"
        };

        string ScoreBadge(double score) => score switch
        {
            >= 70 => "<span style='color:#22c55e;font-weight:bold;'>ممتاز ✨</span>",
            >= 50 => "<span style='color:#3b82f6;font-weight:bold;'>جيد 👍</span>",
            >= 30 => "<span style='color:#f59e0b;font-weight:bold;'>مقبول ⚠️</span>",
            _     => "<span style='color:#ef4444;font-weight:bold;'>يحتاج تحسين 🔔</span>"
        };

        string Tip(SimulationResult r)
        {
            if (r.OverallRiskIndex > 0.65) return "⚠️ <b>تحذير:</b> مستوى المخاطر الكلي مرتفع. ننصح بمراجعة نمط الادخار وساعات الراحة.";
            if (r.BurnoutRisk > 0.6)       return "🔋 <b>انتبه:</b> خطر الإرهاق مرتفع — خذ استراحات منتظمة وخفّف ضغط العمل.";
            if (r.SocialBalanceScore < 30)  return "👥 <b>نصيحة:</b> قضِ المزيد من الوقت مع العائلة والأصدقاء — يحسّن الإنتاجية والسعادة.";
            if (r.LifeStrategyScore > 60)   return "🚀 <b>رائع!</b> استراتيجيتك في الحياة ممتازة — واصل على نفس المسار!";
            return "💡 <b>نصيحة:</b> زيادة ساعات الدراسة أو الادخار بنسبة 5% تُحسّن مسارك بشكل كبير خلال 5 سنوات.";
        }

        var month = DateTime.UtcNow.ToString("MMMM yyyy");
        var hasData = r != null;

        var metricsSection = hasData ? $@"
        <table width='100%' cellpadding='12' cellspacing='0' style='border-collapse:collapse;margin:20px 0;'>
          <tr style='background:#1e293b;color:#fff;text-align:right;'>
            <th style='padding:10px 16px;border-radius:8px 8px 0 0;'>المؤشر</th>
            <th style='padding:10px 16px;'>القيمة</th>
            <th style='padding:10px 16px;'>التقييم</th>
          </tr>
          <tr style='background:#f8fafc;text-align:right;'>
            <td style='padding:10px 16px;border-bottom:1px solid #e2e8f0;'>💰 المدخرات المتوقعة (10 سنوات)</td>
            <td style='padding:10px 16px;border-bottom:1px solid #e2e8f0;font-weight:bold;'>{r!.Savings10Y:N0} {r.Currency}</td>
            <td style='padding:10px 16px;border-bottom:1px solid #e2e8f0;'>{ScoreBadge(r.Savings10Y > 100000 ? 80 : r.Savings10Y > 50000 ? 55 : 30)}</td>
          </tr>
          <tr style='background:#fff;text-align:right;'>
            <td style='padding:10px 16px;border-bottom:1px solid #e2e8f0;'>🏋️ درجة الصحة (السنة الأولى)</td>
            <td style='padding:10px 16px;border-bottom:1px solid #e2e8f0;font-weight:bold;'>{r.HealthScore1Y:F1} / 100</td>
            <td style='padding:10px 16px;border-bottom:1px solid #e2e8f0;'>{ScoreBadge(r.HealthScore1Y)}</td>
          </tr>
          <tr style='background:#f8fafc;text-align:right;'>
            <td style='padding:10px 16px;border-bottom:1px solid #e2e8f0;'>🎯 نسبة استراتيجية الحياة</td>
            <td style='padding:10px 16px;border-bottom:1px solid #e2e8f0;font-weight:bold;'>{r.LifeStrategyScore:F1} / 100</td>
            <td style='padding:10px 16px;border-bottom:1px solid #e2e8f0;'>{ScoreBadge(r.LifeStrategyScore)}</td>
          </tr>
          <tr style='background:#fff;text-align:right;'>
            <td style='padding:10px 16px;border-bottom:1px solid #e2e8f0;'>🔥 خطر الإرهاق (Burnout)</td>
            <td style='padding:10px 16px;border-bottom:1px solid #e2e8f0;font-weight:bold;'>{r.BurnoutRisk * 100:F0}%</td>
            <td style='padding:10px 16px;border-bottom:1px solid #e2e8f0;'>{RiskBadge(r.BurnoutRisk)}</td>
          </tr>
          <tr style='background:#f8fafc;text-align:right;'>
            <td style='padding:10px 16px;'>⚠️ مستوى المخاطر الكلي</td>
            <td style='padding:10px 16px;font-weight:bold;'>{r.OverallRiskIndex * 100:F0}%</td>
            <td style='padding:10px 16px;'>{RiskBadge(r.OverallRiskIndex)}</td>
          </tr>
        </table>
        <div style='background:#fef3c7;border-right:4px solid #f59e0b;padding:16px;border-radius:8px;margin:16px 0;text-align:right;direction:rtl;'>
          {Tip(r)}
        </div>" : @"
        <div style='background:#f1f5f9;padding:20px;border-radius:12px;text-align:center;margin:20px 0;'>
          <p style='color:#64748b;font-size:16px;'>لم تقم بأي محاكاة هذا الشهر بعد 🤔</p>
          <p style='color:#64748b;'>افتح التطبيق وابدأ رحلتك نحو مستقبل أفضل!</p>
        </div>";

        return $@"<!DOCTYPE html>
<html dir='rtl' lang='ar'>
<head><meta charset='UTF-8'><meta name='viewport' content='width=device-width,initial-scale=1.0'></head>
<body style='margin:0;padding:0;background:#f1f5f9;font-family:Arial,sans-serif;direction:rtl;'>
  <table width='100%' cellpadding='0' cellspacing='0'>
    <tr><td align='center' style='padding:40px 20px;'>
      <table width='600' cellpadding='0' cellspacing='0' style='background:#fff;border-radius:16px;overflow:hidden;box-shadow:0 4px 20px rgba(0,0,0,0.08);max-width:100%;'>

        <!-- Header -->
        <tr><td style='background:linear-gradient(135deg,#0f172a,#1e40af);padding:40px;text-align:center;'>
          <h1 style='color:#fff;margin:0;font-size:28px;'>🚀 FutureLife</h1>
          <p style='color:#93c5fd;margin:8px 0 0;font-size:16px;'>تقريرك الشهري — {month}</p>
        </td></tr>

        <!-- Greeting -->
        <tr><td style='padding:32px 40px 0;'>
          <h2 style='color:#1e293b;margin:0 0 8px;font-size:22px;'>مرحباً {user.FullName} 👋</h2>
          <p style='color:#64748b;font-size:15px;margin:0;'>هذا ملخص أحدث محاكاة لمستقبلك. راجع الأرقام واتخذ القرارات الصحيحة!</p>
        </td></tr>

        <!-- Metrics -->
        <tr><td style='padding:20px 40px;'>
          {metricsSection}
        </td></tr>

        <!-- CTA Button -->
        <tr><td style='padding:0 40px 32px;text-align:center;'>
          <a href='https://futurelife.app' style='display:inline-block;background:linear-gradient(135deg,#1e40af,#3b82f6);color:#fff;text-decoration:none;padding:14px 40px;border-radius:50px;font-size:16px;font-weight:bold;'>
            افتح تطبيقك الآن →
          </a>
        </td></tr>

        <!-- Footer -->
        <tr><td style='background:#f8fafc;padding:24px 40px;text-align:center;border-top:1px solid #e2e8f0;'>
          <p style='color:#94a3b8;font-size:13px;margin:0;'>تم إرسال هذا البريد من FutureLife تلقائياً كل شهر.</p>
          <p style='color:#94a3b8;font-size:13px;margin:4px 0 0;'>© {DateTime.UtcNow.Year} FutureLife. جميع الحقوق محفوظة.</p>
        </td></tr>

      </table>
    </td></tr>
  </table>
</body>
</html>";
    }
}
