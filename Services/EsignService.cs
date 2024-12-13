using DocEsignAPI.Models;
using DocEsignAPI.Services;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Signatures;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocEsignAPI.Domain.Services
{
    public class EsignService : IEsignService
    {
        IConfiguration _configuration;
        PdfSigner signer;
        public EsignService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Task<byte[]> EsignDocAsync(EsignProp prop,IFormFile file)
        {
            using var outputStram = new MemoryStream();
            DateTime EsignDate = DateTime.ParseExact(prop.EsignDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime OEsignDate = DateTime.ParseExact(prop.EsignDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            Console.WriteLine("Constant Date: " + DateTime.ParseExact(prop.EsignDate, "dd/MM/yyyy", CultureInfo.InvariantCulture));
            //SignatureDateTime = DateTime.ParseExact(IssueDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime DefaultDate = DateTime.ParseExact("03/03/2022", "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime ODefaultDate = DateTime.ParseExact("19/11/2019", "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime EsignEndDate = DateTime.ParseExact("26/04/2023", "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime OEsignEndDate = DateTime.ParseExact("19/11/2021", "dd/MM/yyyy", CultureInfo.InvariantCulture);
            PdfReader reader;
            PdfSigner signer;

            string KEYSTORE = String.Empty;
            char[] PASSWORD = null;
            if (EsignDate > DefaultDate && EsignDate < EsignEndDate)
            {
                 KEYSTORE = _configuration["Conf:SignaturePath_old"];
                 PASSWORD = _configuration["Conf:SignaturePassword_old"].ToCharArray();
            }
            else if (OEsignDate > ODefaultDate && OEsignDate < OEsignEndDate)
            {
                KEYSTORE = _configuration["Conf:OSignaturePath"];
                PASSWORD = _configuration["Conf:OSignaturePassword"].ToCharArray();
            }
            else
            {
                KEYSTORE = _configuration["Conf:SignaturePath"];
                PASSWORD = _configuration["Conf:SignaturePassword"].ToCharArray();
            }
            
            Pkcs12Store pk12 = new Pkcs12Store(new FileStream(KEYSTORE, FileMode.Open, FileAccess.Read), PASSWORD);
            string alias = null;
            foreach (object a in pk12.Aliases)
            {
                alias = ((string)a);
                if (pk12.IsKeyEntry(alias))
                {
                    break;
                }
            }
            ICipherParameters pk = pk12.GetKey(alias).Key;

            X509CertificateEntry[] ce = pk12.GetCertificateChain(alias);
            X509Certificate[] chain = new X509Certificate[ce.Length];
            for (int k = 0; k < ce.Length; ++k)
            {
                chain[k] = ce[k].Certificate;
            }
            if (file.FileName.StartsWith("M"))
            {
                // Password for PCV
                //string password = "Rqbe@123";
                // Password for GCV,PC,TW
                //string password = "RQBE-6PNM-K9Q6-1R5Q-TWOF";
                string password = _configuration["Conf:AVOPdfPassword"];
                byte[] ownerPassword = Encoding.UTF8.GetBytes(password);
                 reader = new PdfReader(file.OpenReadStream(), new ReaderProperties().SetPassword(ownerPassword));
                StampingProperties stampingProperties = new StampingProperties();
                //stampingProperties.SetAppendMode(true);
                signer = new PdfSigner(reader, outputStram, true);
            }
            else
            {
                 reader = new PdfReader(file.OpenReadStream());
                 signer = new PdfSigner(reader, outputStram, true);
            }

            //string pdfPath = @"D:\Data\M00271618.pdf";

            //PdfReader reader = new PdfReader(pdfPath,new ReaderProperties().SetPassword(ownerPassword));
            


            DateTime date = DateTime.ParseExact(prop.EsignDate, "dd/MM/yyyy", null);
            ////string date = Convert.ToDateTime(prop.EsignDate).ToString("dd/MM/yyyy hh:mm:ss");
            ////DateTime date1 = Convert.ToDateTime(date);
            date = date.AddHours(-5).AddMinutes(-30);
            
            //signer.SetSignDate(date1.Date.AddHours(-5).AddMinutes(-30));
            signer.SetSignDate(date);
            PdfSignatureAppearance appearance = signer.GetSignatureAppearance();
            
            appearance.SetPageRect(new Rectangle(prop.Cordinates.X, prop.Cordinates.Y, prop.Cordinates.Width, prop.Cordinates.Height))
            //appearance.SetPageRect(new Rectangle(130, 100, 80, 80))
                .SetPageNumber(prop.Page_No != 0 ? prop.Page_No : 0 );

            appearance.SetReason(prop.Reason);
            appearance.SetLocation(prop.Location);
            
            appearance.SetSignatureCreator("Raheja Qbe GIC Ltd");
            
            //appearance.SetLayer2Text("layer 2 text");
            //appearance.SetLayer2FontSize(14);
            //appearance.SetLayer2Font(PdfFontFactory.CreateFont(StandardFonts.HELVETICA));

            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CMS);
            return Task.FromResult(outputStram.ToArray());
        }

        
    }
}
