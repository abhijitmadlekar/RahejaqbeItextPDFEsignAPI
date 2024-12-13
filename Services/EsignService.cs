/*
     This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License version 3
    as published by the Free Software Foundation with the addition of the
    following permission added to Section 15 as permitted in Section 7(a):
    FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
    ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
    OF THIRD PARTY RIGHTS
    
    This program is distributed in the hope that it will be useful, but
    WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
    or FITNESS FOR A PARTICULAR PURPOSE.
    See the GNU Affero General Public License for more details.
    You should have received a copy of the GNU Affero General Public License
    along with this program; if not, see http://www.gnu.org/licenses or write to
    the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
    Boston, MA, 02110-1301 USA, or download the license from the following URL:
    http://itextpdf.com/terms-of-use/
    
    The interactive user interfaces in modified source and object code versions
    of this program must display Appropriate Legal Notices, as required under
    Section 5 of the GNU Affero General Public License.
    
    In accordance with Section 7(b) of the GNU Affero General Public License,
    a covered work must retain the producer line in every PDF that is created
    or manipulated using iText.
    
    You can be released from the requirements of the license by purchasing
    a commercial license. Buying such a license is mandatory as soon as you
    develop commercial activities involving the iText software without
    disclosing the source code of your own applications.
    These activities include: offering paid services to customers as an ASP,
    serving PDFs on the fly in a web application, shipping iText with a closed
    source product.
    
    For more information, please contact iText Software Corp. at this
    address: sales@itextpdf.com
 */





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
            DateTime DefaultDate = DateTime.ParseExact("<DefaultDate>", "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime ODefaultDate = DateTime.ParseExact("<ODefaultDate>", "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime EsignEndDate = DateTime.ParseExact("<EsignEndDate>", "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime OEsignEndDate = DateTime.ParseExact("<OEsignEndDate>", "dd/MM/yyyy", CultureInfo.InvariantCulture);
            PdfReader reader;
            PdfSigner signer;

            string KEYSTORE = String.Empty;
            char[] PASSWORD = null;
            
            
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
            
                 reader = new PdfReader(file.OpenReadStream());
                 signer = new PdfSigner(reader, outputStram, true);
            

            
            DateTime date = DateTime.ParseExact(prop.EsignDate, "dd/MM/yyyy", null);

            signer.SetSignDate(date);
            PdfSignatureAppearance appearance = signer.GetSignatureAppearance();
            
            appearance.SetPageRect(new Rectangle(prop.Cordinates.X, prop.Cordinates.Y, prop.Cordinates.Width, prop.Cordinates.Height))
                .SetPageNumber(prop.Page_No != 0 ? prop.Page_No : 0 );

            appearance.SetReason(prop.Reason);
            appearance.SetLocation(prop.Location);
            
            appearance.SetSignatureCreator("Raheja Qbe GIC Ltd");

            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CMS);
            return Task.FromResult(outputStram.ToArray());
        }

        
    }
}
