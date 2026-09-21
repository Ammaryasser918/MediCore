using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Common.Templates
{
    internal class OtpEmailTemplate
    {
        public static string Build(string otpCode, int validityMinutes = 5)
        {
            return $@"
                    <!DOCTYPE html>
                    <html>
                    <body style=""margin:0; padding:0; background-color:#f4f5f7; font-family: Arial, Helvetica, sans-serif;"">
                      <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#f4f5f7; padding:30px 0;"">
                        <tr>
                          <td align=""center"">
                            <table role=""presentation"" width=""420"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#ffffff; border-radius:8px; overflow:hidden; box-shadow:0 1px 4px rgba(0,0,0,0.08);"">
                     
                              <tr>
                                <td style=""background-color:#2563eb; padding:20px 30px;"">
                                  <span style=""color:#ffffff; font-size:20px; font-weight:bold;"">PulseID</span>
                                </td>
                              </tr>
                     
                              <tr>
                                <td style=""padding:30px;"">
                                  <p style=""font-size:15px; color:#333333; margin:0 0 20px;"">
                                    Use the verification code below to complete your request:
                                  </p>
                     
                                  <div style=""background-color:#f0f4ff; border:1px solid #d6e0ff; border-radius:6px; padding:18px; text-align:center; margin-bottom:20px;"">
                                    <span style=""font-size:32px; font-weight:bold; letter-spacing:8px; color:#2563eb;"">{otpCode}</span>
                                  </div>
                     
                                  <p style=""font-size:13px; color:#777777; margin:0 0 8px;"">
                                    This code expires in <strong>{validityMinutes} minutes</strong>.
                                  </p>
                                  <p style=""font-size:13px; color:#777777; margin:0;"">
                                    If you didn't request this code, you can safely ignore this email.
                                  </p>
                                </td>
                              </tr>
                     
                              <tr>
                                <td style=""background-color:#fafafa; padding:15px 30px; text-align:center;"">
                                  <span style=""font-size:12px; color:#aaaaaa;"">&copy; PulseID</span>
                                </td>
                              </tr>
                     
                            </table>
                          </td>
                        </tr>
                      </table>
                    </body>
                    </html>";
        }
    }
}
