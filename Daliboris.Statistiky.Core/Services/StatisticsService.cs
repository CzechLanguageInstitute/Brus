using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net.Mime;
using System.Xml;
using Daliboris.Statistiky.Core.Models.Jevy.XML;
using Daliboris.Word.Text;

namespace Daliboris.Statistiky.Core.Services
{
    public class StatisticsService
    {
        const string CsTypRelaceJevy = @"http://schemata.brus.cz/statisitky/2008/jevy/";
        const string CsTypRelaceSlova = @"http://schemata.brus.cz/statisitky/2008/jevy/slova/";
        const string CsTypRelaceZnaky = @"http://schemata.brus.cz/statisitky/2008/jevy/znaky/";

        public static bool OdstranitTecku = true;

        public string VystupniSoubor { get; set; }

        public SkupinaJevu SkupinaJevu { get; set; }

        /// <summary>
        /// Sloučí detaily jednotlivých jevů za celý dokument
        /// </summary>
        public bool SloucitDetaily { get; set; } = false;


        public void ZpracujStatistiky()
        {
        }

        public static void ExtractFilesForTranscriptorium(string inputFilePath, string outpuDirecotry)
        {
            CheckIfFileExists(inputFilePath);

            SkupinaJevu skj = new SkupinaJevu();

            JevyService jvsJevyService = new JevyService();
            using (Package wdPackage = Package.Open(inputFilePath, FileMode.Open, FileAccess.Read))
            {
                PackageRelationship docPackageRelationship =
                    wdPackage.GetRelationshipsByType(Relace.Statistika).FirstOrDefault();
                if (docPackageRelationship != null)
                {
                    Uri documentUri = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative),
                        docPackageRelationship.TargetUri);
                    PackagePart documentPart = wdPackage.GetPart(documentUri);
                    PackageRelationshipCollection docCollection = documentPart.GetRelationships();

                    int i = 0;
                    foreach (PackageRelationship docRelationship in docCollection)
                    {
                        //List<Jevy> jvs = new List<Jevy>(8);

                        string strRelaceId = docRelationship.Id;

                        PackagePart p = NajdiPackagePart(documentPart, strRelaceId);


                        if (p != null)
                        {
                            PackageRelationshipCollection pcols = p.GetRelationshipsByType(Relace.Slova);
                            foreach (PackageRelationship pr in pcols)
                            {
                                i++;
                                Uri ur = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative), pr.TargetUri);
                                PackagePart prt = wdPackage.GetPart(ur);
                                string outputFilePath = Path.Combine(outpuDirecotry,
                                    String.Format("cs\\{0}\\slova.xml", i));
                                FileInfo fi = new FileInfo(outputFilePath);

                                string filePath = ExtractPart(prt, outpuDirecotry);

                                Directory.CreateDirectory(Path.GetDirectoryName(fi.FullName));
                                if (fi.Exists)
                                    fi.Delete();
                                File.Move(filePath, outputFilePath);
                            }
                        }
                    }
                }

                docPackageRelationship = wdPackage.GetRelationshipsByType(Relace.Obsah).FirstOrDefault();
                if (docPackageRelationship != null)
                {
                    string outputFilePath = Path.Combine(outpuDirecotry, "content.xml");
                    ExtractPart(docPackageRelationship, wdPackage, outpuDirecotry, outputFilePath);
                }


                docPackageRelationship = wdPackage.GetRelationshipsByType(Relace.Styly).FirstOrDefault();
                if (docPackageRelationship != null)
                {
                    string outputFilePath = Path.Combine(outpuDirecotry, "styles.xml");
                    ExtractPart(docPackageRelationship, wdPackage, outpuDirecotry, outputFilePath);
                }
            }
        }


        public static SkupinaJevu NactiStatistiky(string strVstupniSoubor)
        {
            CheckIfFileExists(strVstupniSoubor);

            SkupinaJevu skj = new SkupinaJevu();

            JevyService jvsJevyService = new JevyService();
            using (Package wdPackage = Package.Open(strVstupniSoubor, FileMode.Open, FileAccess.Read))
            {
                PackageRelationship docPackageRelationship =
                    wdPackage.GetRelationshipsByType(Relace.Statistika).FirstOrDefault();
                if (docPackageRelationship != null)
                {
                    Uri documentUri = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative),
                        docPackageRelationship.TargetUri);
                    PackagePart documentPart = wdPackage.GetPart(documentUri);
                    PackageRelationshipCollection docCollection = documentPart.GetRelationships();

                    foreach (PackageRelationship docRelationship in docCollection)
                    {
                        //List<Jevy> jvs = new List<Jevy>(8);

                        string strRelaceId = docRelationship.Id;

                        PackagePart p = NajdiPackagePart(documentPart, strRelaceId);
                        if (p != null)
                        {
                            //PackageRelationshipCollection pcols = p.GetRelationshipsByType(Relace.Slova);
                            PackageRelationshipCollection pcols = p.GetRelationships();
                            foreach (PackageRelationship pr in pcols)
                            {
                                Uri ur = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative), pr.TargetUri);
                                PackagePart prt = wdPackage.GetPart(ur);
                                string s = pr.RelationshipType;
                                Jevy jv = new Jevy();
                                Stream stream = prt.GetStream();
                                if (prt.ContentType == MediaTypeNames.Text.Plain)
                                {
                                    jv = JevyService.NactiJevy(stream);
                                }
                                else if (prt.ContentType == MediaTypeNames.Text.Xml)
                                {
                                    XmlDocument jevy = LoadPackagePartAsXmlDocument(wdPackage, ur);
                                    string path = Path.GetTempFileName();
                                    jevy.Save(path);
                                    jv = JevyService.NactiZeSouboruXml(path);
                                }

                                //jvs.Add(jv);
                                skj.Add(jv);
                            }
                        }
                    }
                }
            }

            return skj;
        }

        public static List<Jevy> NactiObsah(string strVstupniSoubor, string strRelaceId)
        {
            List<Jevy> jvs = new List<Jevy>(8);

            CheckIfFileExists(strVstupniSoubor);

            using (Package wdPackage = Package.Open(strVstupniSoubor, FileMode.Open, FileAccess.Read))
            {
                PackageRelationship docPackageRelationship =
                    wdPackage.GetRelationshipsByType(Relace.Statistika).FirstOrDefault();

                if (docPackageRelationship != null)
                {
                    Uri documentUri = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative),
                        docPackageRelationship.TargetUri);
                    PackagePart documentPart = wdPackage.GetPart(documentUri);
                    PackagePart p = NajdiPackagePart(documentPart, strRelaceId);
                    if (p != null)
                    {
                        PackageRelationshipCollection pcols = p.GetRelationships();
                        foreach (PackageRelationship pr in pcols)
                        {
                            Uri ur = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative), pr.TargetUri);
                            PackagePart prt = wdPackage.GetPart(ur);
                            string s = pr.RelationshipType;
                            Jevy jv = new Jevy();
                            Stream stream = prt.GetStream();
                            if (prt.ContentType == MediaTypeNames.Text.Plain)
                            {
                                jv = JevyService.NactiJevy(stream);
                            }

                            jvs.Add(jv);
                        }
                    }

                    /*
                    PackageRelationshipCollection pkgCols = documentPart.GetRelationships();
                    foreach (PackageRelationship pkgRel in pkgCols)
                    {
                        if (pkgRel.Id != strRelaceId) continue;
                        Uri uri = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative), pkgRel.TargetUri);
                        PackagePart part = wdPackage.GetPart(uri);
                        PackageRelationshipCollection pcols = part.GetRelationships();
                        foreach (PackageRelationship pr in pcols)
                        {
                            Uri ur = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative), pr.TargetUri);
                            PackagePart prt = wdPackage.GetPart(ur);
                            Jevy jv = new Jevy();
                            Stream stream = prt.GetStream();
                            if (prt.ContentType == MediaTypeNames.Text.Plain)
                            {
                                jv = Sprava.NactiJevy(stream);
                            }
                            jvs.Add(jv);
                        }
                    }
                    */
                }
            }

            return jvs;
        }

        private static void CheckIfFileExists(string strVstupniSoubor)
        {
            if (!File.Exists(strVstupniSoubor))
                throw new FileNotFoundException("Soubor '" + strVstupniSoubor + "' nebyl nalezen.");
        }

        private static PackagePart NajdiPackagePart(PackagePart pkpVychozi, string strRelaceId)
        {
            PackageRelationshipCollection pkgCols = pkpVychozi.GetRelationships();
            foreach (PackageRelationship pkgRel in pkgCols)
            {
                if (pkgRel.Id != strRelaceId)
                {
                    Uri uri = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative), pkgRel.TargetUri);
                    PackagePart pkpCast = pkpVychozi.Package.GetPart(uri);
                    PackagePart p = NajdiPackagePart(pkpCast, strRelaceId);
                    if (p != null)
                        return p;
                }
                else
                {
                    Uri uri = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative), pkgRel.TargetUri);
                    PackagePart pkpCast = pkpVychozi.Package.GetPart(uri);
                    return pkpCast;
                }
            }

            return null;
        }

        public static XmlDocument NactiDataStatistiky(string strVstupniSoubor)
        {
            XmlDocument xd = null;
            CheckIfFileExists(strVstupniSoubor);

            using (Package wdPackage = Package.Open(strVstupniSoubor, FileMode.Open, FileAccess.Read))
            {
                PackageRelationship docPackageRelationship =
                    wdPackage.GetRelationshipsByType(Relace.Data).FirstOrDefault();
                if (docPackageRelationship != null)
                {
                    Uri documentUri = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative),
                        docPackageRelationship.TargetUri);
                    xd = LoadPackagePartAsXmlDocument(wdPackage, documentUri);
                }
            }

            return xd;
        }

        private static XmlDocument LoadPackagePartAsXmlDocument(Package wdPackage, Uri documentUri)
        {
            PackagePart documentPart = wdPackage.GetPart(documentUri);
            Stream str = documentPart.GetStream();
            str.Position = 0;
            XmlDocument xd = new XmlDocument();
            xd.Load(str);
            return xd;
        }


        /// <summary>
        /// Uloží skupinu jevů v požadovaném formátu do souboru
        /// </summary>
        /// <param name="skjJevy">Skupina jevů, které se mají uložit</param>
        /// <param name="strVystupniSoubor">Výstupní soubor, do kterého se jevy uloží</param>
        /// <param name="fusFormat">Formát ukládaných seznamů</param>
        public static void UlozStatistiky(SkupinaJevu skjJevy, string strVystupniSoubor, FormatUlozeniSeznamu fusFormat)
        {
            UlozStatistiky(skjJevy, strVystupniSoubor, fusFormat, true);
        }

        /// <summary>
        /// Uloží skupinu jevů v požadovaném formátu do souboru
        /// </summary>
        /// <param name="skjJevy">Skupina jevů, které se mají uložit</param>
        /// <param name="strVystupniSoubor">Výstupní soubor, do kterého se jevy uloží</param>
        /// <param name="fusFormat">Formát ukládaných seznamů</param>
        /// <param name="odstranitTeckuUSlov"></param>
        public static void UlozStatistiky(SkupinaJevu skjJevy, string strVystupniSoubor, FormatUlozeniSeznamu fusFormat,
            bool odstranitTeckuUSlov)
        {
            UlozStatistiky(skjJevy, strVystupniSoubor, fusFormat, null, odstranitTeckuUSlov, null, null);
        }

        public static void UlozStatistiky(SkupinaJevu skjJevy, string strVystupniSoubor,
            FormatUlozeniSeznamu fusFormat, bool odstranitTeckuUSlov, List<string> obsahDokumentu,
            Dictionary<string, int> stylyDokumentu)
        {
            UlozStatistiky(skjJevy, strVystupniSoubor, fusFormat, null, odstranitTeckuUSlov, obsahDokumentu,
                stylyDokumentu);
        }


        /// <summary>
        /// Uloží skupinu jevů v požadovaném formátu do souboru
        /// </summary>
        /// <param name="skjJevy">Skupina jevů, které se mají uložit</param>
        /// <param name="strVystupniSoubor">Výstupní soubor, do kterého se jevy uloží</param>
        /// <param name="fusFormat">Formát ukládaných seznamů</param>
        /// <param name="vlbVlastnosti">Vlastnosti ukládaného souboru, metainformace</param>
        /// <param name="odstranitTeckuUSlov">Zda se má při generování slov z úseků odstranit tečka za slovem.</param>
        /// <param name="obsahDokumentu"></param>
        /// <param name="stylyDokumentu"></param>
        public static void UlozStatistiky(SkupinaJevu skjJevy, string strVystupniSoubor, FormatUlozeniSeznamu fusFormat,
            VlastnostiBalicku vlbVlastnosti, bool odstranitTeckuUSlov, List<string> obsahDokumentu,
            Dictionary<string, int> stylyDokumentu)
        {
            FileInfo fileInfo = new FileInfo(strVystupniSoubor);
            OdstranitTecku = odstranitTeckuUSlov;

            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();

            using (Package package = Package.Open(strVystupniSoubor, FileMode.Create))
            {
                if (vlbVlastnosti == null)
                {
                    vlbVlastnosti = VytvoritVlastnostiBalicek();
                }

                PriraditVlastnostiBalicku(vlbVlastnosti, package);

                string strTemp = Path.GetTempPath();
                Dictionary<TypJevu, SkupinaJevu> gdcSeskupeneJevy = new Dictionary<TypJevu, SkupinaJevu>(skjJevy.Count);


                Uri partUriStatistiky =
                    PackUriHelper.CreatePartUri(new Uri(AdresarovaStruktura.Statistika + "\\statistika.xml",
                        UriKind.Relative));
                PackagePart pkgpStatistiky = package.CreatePart(partUriStatistiky, MediaTypeNames.Text.Xml,
                    CompressionOption.Maximum);

                foreach (Jevy kvp in skjJevy)
                {
                    if (!gdcSeskupeneJevy.ContainsKey(kvp.Druh))
                    {
                        gdcSeskupeneJevy.Add(kvp.Druh, new SkupinaJevu());
                    }

                    gdcSeskupeneJevy[kvp.Druh].Add(kvp);
                }

                List<MetadataOJevu> glsOdstavcoveStyly = new List<MetadataOJevu>();
                List<MetadataOJevu> glsZnakoveStyly = new List<MetadataOJevu>();

                Jevy vsechnyZnaky = new Jevy(TypJevu.Znaky);
                vsechnyZnaky.Identifikator = "vsechnyZnaky";

                string strPripona = PriponaPodleFormatuUlozeni(fusFormat);
                foreach (KeyValuePair<TypJevu, SkupinaJevu> kvp in gdcSeskupeneJevy)
                {
                    #region Odstavce

                    if (kvp.Key == TypJevu.Odstavce)
                    {
                        //TODO Vysat informace o kolekci do seznamu statistik
                        //TODO Uložit soubor v požadovaném formátu do adresáře OdstavcoveStyly
                        //TODO Vytvořit k danému stylu seznam slov a znaků a ty pak uložit v podadresáři OdstavcovyStylXY/Obsah
                        //TODO Najít k danému stylu podřízené znakové styly a pro ně pak vytvořit samostatnou strukturu, s výchozím adresářem OdstavcovyStylXY/ZnakoveStyly
                        int i = 0;
                        foreach (Jevy jvs in kvp.Value)
                        {
                            MetadataOJevu mtdOdstavcovyStyl = ZiskatMetadataOJevu(++i, jvs);


                            string strJevyTmp = jvs.ID + strPripona;
                            string strSouborJevyTmp = strTemp + strJevyTmp;
                            string strObsah = "useky" + strPripona;

                            if (fusFormat == FormatUlozeniSeznamu.Xml)
                            {
                                JevyService.UlozDoSouboru(jvs, strSouborJevyTmp, FormatUlozeni.Xml);
                            }

                            if (fusFormat == FormatUlozeniSeznamu.Text)
                            {
                                //TODO Využít CteckuSouboru pro vytvoření textové podoby XML
                                JevyService.UlozDoSouboru(jvs, strSouborJevyTmp, FormatUlozeni.Text);
                            }

                            string strStylId = AdresarovaStruktura.Styl + i.ToString("000");
                            string strSlozkaStylu = AdresarovaStruktura.Statistika + "\\" +
                                                    AdresarovaStruktura.OdstavcoveStyly + "\\" + strStylId;

                            Uri partUriObsah = PackUriHelper.CreatePartUri(new Uri(
                                strSlozkaStylu + "\\" + AdresarovaStruktura.Obsah + "\\" + strObsah, UriKind.Relative));

                            //Uri partUriStyl = PackUriHelper.CreatePartUri(new Uri(AdresarovaStruktura.Statistika + "\\" + AdresarovaStruktura.OdstavcoveStyly + "\\" + strStylId + ".xml", UriKind.Relative));
                            Uri partUriStyl = PackUriHelper.CreatePartUri(new Uri(
                                AdresarovaStruktura.Statistika + "\\" + AdresarovaStruktura.OdstavcoveStyly + "\\" +
                                strStylId + ".xml", UriKind.Relative));

                            PackagePart pkgpStyl = package.CreatePart(partUriStyl, MediaTypeNames.Text.Xml,
                                CompressionOption.Maximum);
                            //TODO Vytvořit dokument XML s informacemi o stylu (nejspíš přebrat vše z hlavičky)


                            PackagePart pkgpObsah = package.CreatePart(partUriObsah,
                                fusFormat == FormatUlozeniSeznamu.Xml
                                    ? MediaTypeNames.Text.Xml
                                    : MediaTypeNames.Text.Plain, CompressionOption.Maximum);


                            using (FileStream fileStream =
                                new FileStream(strSouborJevyTmp, FileMode.Open, FileAccess.Read))
                            {
                                CopyStream(fileStream, pkgpObsah.GetStream());
                            }

                            File.Delete(strSouborJevyTmp);
                            PackageRelationship rel = pkgpStatistiky.CreateRelationship(pkgpStyl.Uri,
                                TargetMode.Internal, Relace.OdstavcovyStyl);
                            mtdOdstavcovyStyl.MetadataBalicku.RelaceId = rel.Id;

                            rel = pkgpStyl.CreateRelationship(pkgpObsah.Uri, TargetMode.Internal, Relace.Useky);
                            mtdOdstavcovyStyl.Useky.RelaceId = rel.Id;


                            /*
                            Jevy jvSlova;
                            Jevy jvZnaky;
                            RozdelitUsekNaSlovaAZnaky(jvs, out jvSlova, out jvZnaky);
                            Jevy jvTrigramy;
                            Jevy jvDigramy;
                            RozdelitSlovaNaNgramy(jvSlova, out jvDigramy, 2);
                            RozdelitSlovaNaNgramy(jvSlova, out jvTrigramy, 3);

                            mtdOdstavcovyStyl.PocetSlov.Jedinecnych = jvSlova.Pocet;
                            mtdOdstavcovyStyl.PocetSlov.Celkem = jvSlova.SoucetJevu();

                            mtdOdstavcovyStyl.PocetZnaku.Jedinecnych = jvZnaky.Pocet;
                            mtdOdstavcovyStyl.PocetZnaku.Celkem = jvZnaky.SoucetJevu();
                            */

                            Jevy jvSlova;
                            Jevy jvZnaky;
                            Jevy jvTrigramy;
                            Jevy jvDigramy;

                            RozdelitUsekNaSlovaAZnaky(jvs, out jvSlova, out jvZnaky);
                            RozdelitSlovaNaNgramy(jvSlova, out jvDigramy, 2);
                            RozdelitSlovaNaNgramy(jvSlova, out jvTrigramy, 3);

                            SloucitStatistikyZnaku(jvZnaky, vsechnyZnaky);

                            PriraditPoctyDoMetadat(mtdOdstavcovyStyl.PocetSlov, jvSlova);
                            PriraditPoctyDoMetadat(mtdOdstavcovyStyl.PocetZnaku, jvZnaky);

                            PriraditPoctyDoMetadat(mtdOdstavcovyStyl.PocetDigramu, jvDigramy);
                            PriraditPoctyDoMetadat(mtdOdstavcovyStyl.PocetTrigramu, jvTrigramy);

                            rel = UlozitJevyDoBalickuJakoObsah(fusFormat, strPripona, strJevyTmp,
                                AdresarovaStruktura.Statistika + "\\", AdresarovaStruktura.OdstavcoveStyly,
                                strStylId, package, pkgpStyl, jvDigramy, "digramy", Relace.Digramy);
                            mtdOdstavcovyStyl.Digramy.RelaceId = rel.Id;

                            rel = UlozitJevyDoBalickuJakoObsah(fusFormat, strPripona, strJevyTmp,
                                AdresarovaStruktura.Statistika + "\\", AdresarovaStruktura.OdstavcoveStyly, strStylId,
                                package, pkgpStyl, jvTrigramy, "trigramy", Relace.Trigramy);
                            mtdOdstavcovyStyl.Trigramy.RelaceId = rel.Id;


                            rel = UlozitJevyDoBalickuJakoObsah(fusFormat, strPripona, strJevyTmp,
                                AdresarovaStruktura.Statistika + "\\", AdresarovaStruktura.OdstavcoveStyly, strStylId,
                                package, pkgpStyl, jvSlova, "slova", Relace.Slova);
                            mtdOdstavcovyStyl.Slova.RelaceId = rel.Id;

                            rel = UlozitJevyDoBalickuJakoObsah(fusFormat, strPripona, strJevyTmp,
                                AdresarovaStruktura.Statistika + "\\",
                                AdresarovaStruktura.OdstavcoveStyly, strStylId, package, pkgpStyl, jvZnaky, "znaky",
                                Relace.Znaky);
                            mtdOdstavcovyStyl.Znaky.RelaceId = rel.Id;


                            List<MetadataOJevu> glsOdstavecZnak = new List<MetadataOJevu>();

                            foreach (KeyValuePair<TypJevu, SkupinaJevu> kvpz in gdcSeskupeneJevy)
                            {
                                if (kvpz.Key == TypJevu.Useky)
                                {
                                    int iPoradi = 0;
                                    foreach (Jevy jv in kvpz.Value)
                                    {
                                        if (jvs.Identifikator != jv.Identifikator &&
                                            jv.Identifikator.StartsWith(jvs.Identifikator))
                                        {
                                            MetadataOJevu mtj = ZiskatMetadataOJevu(++iPoradi, jv);
                                            mtj.Popis = jv.Popis.Substring(jv.Popis.IndexOf('+') + 1).Trim();
                                            UlozitZnakovyStyl(jv, package, pkgpStyl, strSlozkaStylu + "\\", fusFormat,
                                                iPoradi, strTemp, ref mtj);
                                            glsOdstavecZnak.Add(mtj);
                                        }
                                    }
                                }
                            }

                            UlozitOdstavcovyStylDoBalickuJakoObsah(mtdOdstavcovyStyl, glsOdstavecZnak, pkgpStyl);
                            mtdOdstavcovyStyl.PodrizeneJevy = glsOdstavecZnak;
                            glsOdstavcoveStyly.Add(mtdOdstavcovyStyl);
                        }


                        // end:using(fileStream) - Close and dispose fileStream.
                    }

                    #endregion

                    if (kvp.Key == TypJevu.Useky)
                    {
                        int i = 0;
                        foreach (Jevy jvs in kvp.Value)
                        {
                            if (jvs.Identifikator.Contains("+"))
                                continue;
                            //Uloží se pouze samostatné znakové styly, tj. takové, které nejsou součástí odstavcového stylu
                            MetadataOJevu mtj = ZiskatMetadataOJevu(++i, jvs);
                            UlozitZnakovyStyl(jvs, package, pkgpStatistiky, AdresarovaStruktura.Statistika + "\\",
                                fusFormat, i, strTemp, ref mtj);
                            glsZnakoveStyly.Add(mtj);
                        }
                    }
                }
                //MetadataOJevu vsechnyZnakyMetadata = new MetadataOJevu();

                //foreach (MetadataOJevu znakovyStyl in glsZnakoveStyly)
                //{

                //}

                UlozitStatistikuDoBalickuJakoObsahVsechnyZnaky(fusFormat, package, strPripona, vsechnyZnaky);

                //TODO Zapsat statistiku
                UlozitStatistikuDoBalickuJakoObsah(glsOdstavcoveStyly, glsZnakoveStyly, pkgpStatistiky, vlbVlastnosti);
                //TODO Uložit informační soubor pro WPF

                UlozitDoBalickuJakoObsahObsahDokumentu(fusFormat, package, strPripona, obsahDokumentu);

                UlozitDoBalickuJakoObsahStylyDokumentu(fusFormat, package, strPripona, stylyDokumentu);

                Uri partUriData =
                    PackUriHelper.CreatePartUri(
                        new Uri(AdresarovaStruktura.Statistika + "\\data.xml", UriKind.Relative));
                PackagePart pkgpData =
                    package.CreatePart(partUriData, MediaTypeNames.Text.Xml, CompressionOption.Maximum);

                UlozitStatistikyDoBalickuJakoPrehled(glsOdstavcoveStyly, glsZnakoveStyly, pkgpData, vlbVlastnosti);
            }
        }

        private static void UlozitStatistikuDoBalickuJakoObsahVsechnyZnaky(FormatUlozeniSeznamu fusFormat,
            Package package,
            string strPripona, Jevy vsechnyZnaky)
        {
            Uri partUriStyl =
                PackUriHelper.CreatePartUri(
                    new Uri(
                        AdresarovaStruktura.Statistika + "\\" + AdresarovaStruktura.Znaky + "\\" + "vse" + ".xml",
                        UriKind.Relative));
            PackagePart pkgpStyl = package.CreatePart(partUriStyl, MediaTypeNames.Text.Xml, CompressionOption.Maximum);

            PackageRelationship rel2 = UlozitJevyDoBalickuJakoObsah(fusFormat,
                strPripona,
                vsechnyZnaky.ID + strPripona,
                AdresarovaStruktura.Statistika + "\\",
                AdresarovaStruktura.Znaky,
                "vse",
                package,
                pkgpStyl,
                vsechnyZnaky,
                "znaky",
                Relace.Znaky);
        }

        private static void UlozitDoBalickuJakoObsahObsahDokumentu(FormatUlozeniSeznamu fusFormat, Package package,
            string strPripona, List<string> obsahDokumentu)
        {
            if (obsahDokumentu == null || obsahDokumentu.Count == 0)
                return;
            Uri partUriStyl =
                PackUriHelper.CreatePartUri(
                    new Uri(
                        AdresarovaStruktura.Obsah + "\\" + "vse" + ".xml",
                        UriKind.Relative));
            PackagePart pkgpStyl = package.CreatePart(partUriStyl, MediaTypeNames.Text.Xml, CompressionOption.Maximum);

            string tmpSlozka = Path.GetTempPath();


            string souborObsahu = Path.Combine(tmpSlozka, AdresarovaStruktura.Obsah + ".xml");
            using (StreamWriter sw = new StreamWriter(souborObsahu))
            {
                foreach (string odstavec in obsahDokumentu)
                {
                    sw.WriteLine(odstavec);
                }
            }


            PackageRelationship rel2 = UlozitSouborDoBalickuJakoObsah(fusFormat,
                strPripona,
                tmpSlozka,
                AdresarovaStruktura.Obsah + "\\",
                AdresarovaStruktura.Obsah,
                package,
                null,
                "obsah",
                Relace.Obsah);

            //PackageRelationship rel = pkgpStatistiky.Package.CreateRelationship(pkgpStatistiky.Uri, TargetMode.Internal, Relace.Data);
            //
            //package.CreateRelationship(partUriStyl, TargetMode.Internal, Relace.Obsah);
        }

        private static void UlozitDoBalickuJakoObsahStylyDokumentu(FormatUlozeniSeznamu fusFormat, Package package,
            string strPripona, Dictionary<string, int> stylyDokumentu)
        {
            if (stylyDokumentu == null || stylyDokumentu.Count == 0)
                return;

            Dictionary<string, int> styly = stylyDokumentu;

            string tmpSlozka = Path.GetTempPath();
            string souborObsahu = Path.Combine(tmpSlozka, AdresarovaStruktura.Styl + ".xml");

            string souborStylu = souborObsahu;
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            using (XmlWriter xw = XmlWriter.Create(souborStylu, settings))
            {
                xw.WriteStartDocument(true);
                xw.WriteStartElement("styles");
                foreach (KeyValuePair<string, int> kvp in styly)
                {
                    xw.WriteStartElement("style");
                    xw.WriteAttributeString("id", kvp.Value.ToString(CultureInfo.InvariantCulture));
                    xw.WriteAttributeString("name", kvp.Key);
                    xw.WriteEndElement(); //style
                }

                xw.WriteEndElement(); //styles
                xw.WriteEndDocument();
            }


            Uri partUri = new Uri(
                AdresarovaStruktura.Obsah + "\\" + "vse" + ".xml",
                UriKind.Relative);

            Uri partUriStyl = PackUriHelper.CreatePartUri(partUri);


            //PackagePart pkgpStyl = package.CreatePart(partUriStyl, MediaTypeNames.Text.Xml, CompressionOption.Maximum);
            PackagePart pkgpStyl = package.GetPart(partUriStyl);

            PackageRelationship rel2 = UlozitSouborDoBalickuJakoObsah(fusFormat,
                strPripona,
                tmpSlozka,
                AdresarovaStruktura.Obsah + "\\",
                AdresarovaStruktura.Styl,
                package,
                null,
                "styl",
                Relace.Styly);
        }

        private static void SloucitStatistikyZnaku(Jevy jevy, Jevy vsechnyZnaky)
        {
            foreach (Jev jev in jevy)
            {
                if (vsechnyZnaky.Contains(jev))
                {
                    vsechnyZnaky[jev.Nazev].Pocet += jev.Pocet;
                }
                else
                {
                    vsechnyZnaky.Add(jev);
                }
            }
        }

        private static void RozdelitSlovaNaNgramy(Jevy jvSlova, out Jevy jvDigramy, byte btPocetZnaku)
        {
            jvDigramy = new Jevy(TypJevu.NGramy);
            jvDigramy.Identifikator = jvSlova.Identifikator;
            jvDigramy.Popis = jvSlova.Popis;
            jvDigramy.Zdroj = jvSlova.Zdroj;
            jvDigramy.Jazyk = jvSlova.Jazyk;

            foreach (Jev jv in jvSlova)
            {
                NGramy ngs = NGramy.ZpracujNGramy(jv.Nazev, btPocetZnaku);
                foreach (NGram ng in ngs)
                {
                    jvDigramy.Add(new Jev(jv.Jazyk, new string(ng.Znaky)));
                }
            }
        }


        private static string PriponaPodleFormatuUlozeni(FormatUlozeniSeznamu fusFormat)
        {
            string strPripona;
            switch (fusFormat)
            {
                case FormatUlozeniSeznamu.Text:
                    strPripona = ".txt";
                    break;
                default:
                    strPripona = ".xml";
                    break;
            }

            return strPripona;
        }

        private static MetadataOJevu ZiskatMetadataOJevu(int i, Jevy jvs)
        {
            MetadataOJevu metadataOJevu = new MetadataOJevu();
            metadataOJevu.Poradi = i;
            metadataOJevu.Popis = jvs.Popis;
            metadataOJevu.Jazyk = jvs.Jazyk;
            metadataOJevu.PocetUseku.Jedinecnych = jvs.Pocet;
            metadataOJevu.PocetUseku.Celkem = jvs.SoucetJevu();
            metadataOJevu.PosledniZmena = jvs.PosledniZmena;
            metadataOJevu.Zdroj = jvs.Zdroj.CelaCesta;

            return metadataOJevu;
        }

        private static void UlozitZnakovyStyl(Jevy jvJevy, Package package, PackagePart pkgpVychoziCastBalicku,
            string strVychoziSlozka, FormatUlozeniSeznamu fusFormat, int intPoradi, string strDocasnaSlozka,
            ref MetadataOJevu mtdMetadata)
        {
            string strPripona = PriponaPodleFormatuUlozeni(fusFormat);
            string strJevyTmp = jvJevy.ID + strPripona;
            string strSouborJevyTmp = strDocasnaSlozka + strJevyTmp;
            string strObsah = "useky" + strPripona;

            if (fusFormat == FormatUlozeniSeznamu.Xml)
            {
                JevyService.UlozDoSouboru(jvJevy, strSouborJevyTmp, FormatUlozeni.Xml);
            }

            if (fusFormat == FormatUlozeniSeznamu.Text)
            {
                //TODO Využít CteckuSouboru pro vytvoření textové podoby XML
                JevyService.UlozDoSouboru(jvJevy, strSouborJevyTmp, FormatUlozeni.Text);
            }

            string strStylId = AdresarovaStruktura.Styl + intPoradi.ToString("000");

            Uri partUriObsah = PackUriHelper.CreatePartUri(new Uri(
                strVychoziSlozka + AdresarovaStruktura.ZnakoveStyly + "\\" + strStylId + "\\" +
                AdresarovaStruktura.Obsah + "\\" + strObsah, UriKind.Relative));

            Uri partUriStyl = PackUriHelper.CreatePartUri(new Uri(
                strVychoziSlozka + AdresarovaStruktura.ZnakoveStyly + "\\" + strStylId + ".xml", UriKind.Relative));

            PackagePart pkgpStyl = package.CreatePart(partUriStyl, MediaTypeNames.Text.Xml, CompressionOption.Maximum);


            PackagePart pkgpObsah = package.CreatePart(partUriObsah,
                fusFormat == FormatUlozeniSeznamu.Xml ? MediaTypeNames.Text.Xml : MediaTypeNames.Text.Plain,
                CompressionOption.Maximum);


            using (FileStream fileStream = new FileStream(strSouborJevyTmp, FileMode.Open, FileAccess.Read))
            {
                CopyStream(fileStream, pkgpObsah.GetStream());
            }

            File.Delete(strSouborJevyTmp);


            PackageRelationship rel =
                pkgpVychoziCastBalicku.CreateRelationship(pkgpStyl.Uri, TargetMode.Internal, Relace.ZnakovyStyl);
            mtdMetadata.MetadataBalicku.RelaceId = rel.Id;

            rel = pkgpStyl.CreateRelationship(pkgpObsah.Uri, TargetMode.Internal, Relace.Useky);
            mtdMetadata.Useky.RelaceId = rel.Id;

            Jevy jvSlova;
            Jevy jvZnaky;
            Jevy jvTrigramy;
            Jevy jvDigramy;

            RozdelitUsekNaSlovaAZnaky(jvJevy, out jvSlova, out jvZnaky);
            RozdelitSlovaNaNgramy(jvSlova, out jvDigramy, 2);
            RozdelitSlovaNaNgramy(jvSlova, out jvTrigramy, 3);

            PriraditPoctyDoMetadat(mtdMetadata.PocetSlov, jvSlova);
            PriraditPoctyDoMetadat(mtdMetadata.PocetZnaku, jvZnaky);

            PriraditPoctyDoMetadat(mtdMetadata.PocetDigramu, jvDigramy);
            PriraditPoctyDoMetadat(mtdMetadata.PocetTrigramu, jvTrigramy);

            rel = UlozitJevyDoBalickuJakoObsah(fusFormat, strPripona, strJevyTmp, strVychoziSlozka,
                AdresarovaStruktura.ZnakoveStyly, strStylId, package, pkgpStyl, jvDigramy, "digramy", Relace.Digramy);
            mtdMetadata.Digramy.RelaceId = rel.Id;

            rel = UlozitJevyDoBalickuJakoObsah(fusFormat, strPripona, strJevyTmp, strVychoziSlozka,
                AdresarovaStruktura.ZnakoveStyly, strStylId, package, pkgpStyl, jvTrigramy, "trigramy",
                Relace.Trigramy);
            mtdMetadata.Trigramy.RelaceId = rel.Id;


            rel = UlozitJevyDoBalickuJakoObsah(fusFormat, strPripona, strJevyTmp, strVychoziSlozka,
                AdresarovaStruktura.ZnakoveStyly, strStylId, package, pkgpStyl, jvSlova, "slova", Relace.Slova);
            mtdMetadata.Slova.RelaceId = rel.Id;

            rel = UlozitJevyDoBalickuJakoObsah(fusFormat, strPripona, strJevyTmp, strVychoziSlozka,
                AdresarovaStruktura.ZnakoveStyly, strStylId, package, pkgpStyl, jvZnaky, "znaky", Relace.Znaky);
            mtdMetadata.Znaky.RelaceId = rel.Id;

            //TODO Vytvořit dokument XML s informacemi o stylu (přebrat z metadat), uložit do pkgStyl


            UlozitStylDoBalickuJakoObsah(mtdMetadata, pkgpStyl);
        }

        private static void PriraditPoctyDoMetadat(Pocty pcPocty, Jevy jvDigramy)
        {
            pcPocty.Jedinecnych = jvDigramy.Pocet;
            pcPocty.Celkem = jvDigramy.SoucetJevu();
        }

        private static void UlozitStatistikuDoBalickuJakoObsah(IEnumerable<MetadataOJevu> glsOdstavcoveStyly,
            IEnumerable<MetadataOJevu> glsZnakoveStyly,
            PackagePart pkgpStatistiky, VlastnostiBalicku vlbVlastnosti)
        {
            XmlDocument xd = new XmlDocument();

            XmlNamespaceManager xnmsp = new XmlNamespaceManager(xd.NameTable);
            xnmsp.AddNamespace("r", JmenneProstory.Relationship);
            xnmsp.AddNamespace("", JmenneProstory.Statistiky);

            XmlNode xn = xd.CreateElement("statistika");

            XmlAttribute xa = xd.CreateAttribute("xmlns");
            xa.Value = JmenneProstory.Statistiky;
            if (xn.Attributes != null) xn.Attributes.Append(xa);

            xa = xd.CreateAttribute("xmlns:r");
            xa.Value = JmenneProstory.Relationship;
            if (xn.Attributes != null) xn.Attributes.Append(xa);

            //TODO Doplnit informace o zdroji, datu vytvoření ap.; převzít údaje z metadat libivolného znakového stylu
            //TODO Ošetřit případy, kdy není k dispozici žádný znakový styl (využít odstavcové nebo vlastnosti balíčku)
            XmlNode xnz = VytvoritUzelZdroje(xd, glsZnakoveStyly);
            if (xnz == null)
                xnz = VytvoritUzelZdroje(xd, glsOdstavcoveStyly);
            if (xnz != null)
                xn.AppendChild(xnz);


            XmlNode xnos = xd.CreateElement(ZnackyXml.OdstavcoveStyly);
            foreach (MetadataOJevu mtd in glsOdstavcoveStyly)
            {
                XmlNode xns =
                    MetadataOJevu.VytvoritTagZMetadat(xd, mtd.MetadataBalicku, "styl", mtd.Popis, mtd.PocetUseku);
                xnos.AppendChild(xns);
            }

            xn.AppendChild(xnos);

            XmlNode xnzs = xd.CreateElement(ZnackyXml.ZnakoveStyly);
            foreach (MetadataOJevu mtd in glsZnakoveStyly)
            {
                XmlNode xns =
                    MetadataOJevu.VytvoritTagZMetadat(xd, mtd.MetadataBalicku, "styl", mtd.Popis, mtd.PocetUseku);
                xnzs.AppendChild(xns);
            }

            xn.AppendChild(xnzs);
            xd.AppendChild(xn);

            if (xd.DocumentElement != null)
                xd.DocumentElement.AppendChild(xnzs);
            xd.Save(pkgpStatistiky.GetStream());
            PackageRelationship rel =
                pkgpStatistiky.Package.CreateRelationship(pkgpStatistiky.Uri, TargetMode.Internal, Relace.Statistika);
        }

        private static XmlNode VytvoritUzelZdroje(XmlDocument xd, IEnumerable<MetadataOJevu> glsStyly)
        {
            XmlAttribute xa;
            XmlNode xnz = null;
            foreach (MetadataOJevu mtd in glsStyly)
            {
                xnz = xd.CreateElement("zdroj");
                xa = xd.CreateAttribute("cesta");
                xa.Value = mtd.Zdroj;
                xnz.Attributes.Append(xa);

                xa = xd.CreateAttribute("posledniZmena");
                xa.Value = mtd.PosledniZmena.ToString();
                xnz.Attributes.Append(xa);
                break;
            }

            return xnz;
        }

        private static void UlozitStatistikyDoBalickuJakoPrehled(IEnumerable<MetadataOJevu> glsOdstavcoveStyly,
            IEnumerable<MetadataOJevu> glsZnakoveStyly,
            PackagePart pkgpStatistiky, VlastnostiBalicku vlbVlastnosti)
        {
            XmlDocument xd = new XmlDocument();
            XmlDeclaration xdc = xd.CreateXmlDeclaration("1.0", "UTF-8", "yes");
            xd.AppendChild(xdc);
            XmlNode xnss = xd.CreateElement("statistiky");
            XmlNode xns = xd.CreateElement("statistika");
            XmlAttribute xa;

            XmlNode xnz = VytvoritUzelZdroje(xd, glsZnakoveStyly);
            if (xnz == null)
                xnz = VytvoritUzelZdroje(xd, glsOdstavcoveStyly);
            if (xnz != null)
            {
                xa = xd.CreateAttribute("nazev");
                xa.Value = xnz.Attributes["cesta"].Value;
                xns.Attributes.Append(xa);
                xns.AppendChild(xnz);
            }

            UlozitStylyDoBalickuJakoPrehled(glsOdstavcoveStyly, xns, "Odstavcové styly");

            UlozitStylyDoBalickuJakoPrehled(glsZnakoveStyly, xns, "Znakove styly");
            xnss.AppendChild(xns);

            xd.AppendChild(xnss);

            xd.Save(pkgpStatistiky.GetStream());
            PackageRelationship rel =
                pkgpStatistiky.Package.CreateRelationship(pkgpStatistiky.Uri, TargetMode.Internal, Relace.Data);
        }

        private static void UlozitObsahStyluJakoPrehled(XmlNode xno, Pocty pcUseky, string strNazev)
        {
            XmlAttribute xa;
            XmlDocument xd = xno.OwnerDocument;

            XmlNode xnu = xd.CreateElement(strNazev);
            xa = xd.CreateAttribute("celkem");
            xa.Value = pcUseky.Celkem.ToString();
            xnu.Attributes.Append(xa);

            xa = xd.CreateAttribute("jedinecne");
            xa.Value = pcUseky.Jedinecnych.ToString();
            xnu.Attributes.Append(xa);

            xno.AppendChild(xnu);
        }

        private static void UlozitStylyDoBalickuJakoPrehled(IEnumerable<MetadataOJevu> glsStyly, XmlNode xns,
            string strNazev)
        {
            XmlDocument xd = xns.OwnerDocument;
            XmlNode xnp = xns;
            //Pokud je název null, pak se nadřízený uzel (Odstavcové styly, Znakové styly ap.) nevytváří
            if (strNazev != null)
            {
                Pocty pcUseky = new Pocty();
                Pocty pcSlova = new Pocty();
                Pocty pcZnaky = new Pocty();
                Pocty pcDigramy = new Pocty();
                Pocty pcTrigramy = new Pocty();
                foreach (MetadataOJevu mjv in glsStyly)
                {
                    pcUseky.Celkem += mjv.PocetUseku.Celkem;
                    pcUseky.Jedinecnych += mjv.PocetUseku.Jedinecnych;
                    pcSlova.Celkem += mjv.PocetSlov.Celkem;
                    pcSlova.Jedinecnych += mjv.PocetSlov.Jedinecnych;
                    pcZnaky.Celkem += mjv.PocetZnaku.Celkem;
                    pcZnaky.Jedinecnych += mjv.PocetZnaku.Jedinecnych;
                    pcTrigramy.Celkem += mjv.PocetTrigramu.Celkem;
                    pcTrigramy.Jedinecnych += mjv.PocetTrigramu.Jedinecnych;
                    pcDigramy.Celkem += mjv.PocetDigramu.Celkem;
                    pcDigramy.Jedinecnych += mjv.PocetDigramu.Jedinecnych;
                }


                xnp = xd.CreateElement("polozka");
                XmlAttribute xa = xd.CreateAttribute("nazev");
                xa.Value = strNazev;
                xnp.Attributes.Append(xa);


                XmlNode xno = xd.CreateElement("obsah");
                UlozitObsahStyluJakoPrehled(xno, pcUseky, "useky");
                UlozitObsahStyluJakoPrehled(xno, pcSlova, "slova");
                UlozitObsahStyluJakoPrehled(xno, pcZnaky, "znaky");
                UlozitObsahStyluJakoPrehled(xno, pcDigramy, "digramy");
                UlozitObsahStyluJakoPrehled(xno, pcTrigramy, "trigramy");
                xnp.AppendChild(xno);
            }

            //TODO Přidat obsah odstavcových stylů
            foreach (MetadataOJevu mtd in glsStyly)
            {
                XmlNode xn = MetadataOJevu.VytvoritTagZMetadatProPrehled(xd, mtd.MetadataBalicku, "polozka", mtd.Popis,
                    mtd.PocetUseku, mtd.PocetSlov, mtd.PocetZnaku, mtd.PocetDigramu, mtd.PocetTrigramu);
                if (mtd.PodrizeneJevy.Count > 0)
                {
                    UlozitStylyDoBalickuJakoPrehled(mtd.PodrizeneJevy, xn, null);
                }

                xnp.AppendChild(xn);
            }

            if (strNazev != null)
                xns.AppendChild(xnp);
        }

        private static void UlozitOdstavcovyStylDoBalickuJakoObsah(MetadataOJevu mtdMetadata,
            List<MetadataOJevu> glsZnakoveStyly, PackagePart pkgpStyl)
        {
            XmlDocument xd = MetadataOJevu.JakoXmlDocument(mtdMetadata);
            XmlNode xnzs = xd.CreateElement(ZnackyXml.ZnakoveStyly);
            foreach (MetadataOJevu mtd in glsZnakoveStyly)
            {
                XmlNode xns =
                    MetadataOJevu.VytvoritTagZMetadat(xd, mtd.MetadataBalicku, "styl", mtd.Popis, mtd.PocetUseku);
                xnzs.AppendChild(xns);
            }

            if (xd.DocumentElement != null)
                xd.DocumentElement.AppendChild(xnzs);
            xd.Save(pkgpStyl.GetStream());
        }

        private static void UlozitStylDoBalickuJakoObsah(MetadataOJevu mtdMetadata, PackagePart pkgpStyl)
        {
            XmlDocument xd = MetadataOJevu.JakoXmlDocument(mtdMetadata);
            xd.Save(pkgpStyl.GetStream());
        }

        private static void RozdelitUsekNaSlovaAZnaky(Jevy jvJevy, out Jevy jvSlova, out Jevy jvZnaky)
        {
            jvSlova = new Jevy(TypJevu.Slova, jvJevy.Zdroj.CelaCesta, jvJevy.Jazyk, jvJevy.Identifikator, jvJevy.Popis);
            jvZnaky = new Jevy(TypJevu.Znaky, jvJevy.Zdroj.CelaCesta, jvJevy.Jazyk, jvJevy.Identifikator, jvJevy.Popis);

            foreach (Jev jv in jvJevy)
            {
                string sText = jv.Nazev;
                try
                {
                    string[] asSlova = Slova.RozdelitTextNaSlova(sText, OdstranitTecku);
                    foreach (string sSlovo in asSlova)
                    {
                        Jev j = new Jev(jv.Jazyk, sSlovo, null, Slovo.Retrograd(sSlovo), jv.Pocet);
                        foreach (string kontext in jv.Kontexty)
                        {
                            if (!j.Kontexty.Contains(kontext.Trim()))
                                j.Kontexty.Add(kontext.Trim());
                        }

                        jvSlova.Add(j);
                        //if (jvVsechnaSlova != null)
                        //  jvVsechnaSlova.Add(j);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(@"{0} ({1})", sText, exception.Message);
                    throw;
                }

                foreach (char ch in sText)
                {
                    Jev j = new Jev(jv.Jazyk, ch.ToString(), null, jv.Pocet);
                    jvZnaky.Add(j);
                    //if (jvVsechnyZnaky != null)
                    //  jvVsechnyZnaky.Add(j);
                }
            }
        }


        private static PackageRelationship UlozitSouborDoBalickuJakoObsah(FormatUlozeniSeznamu fusFormat,
            string strPripona,
            string tmpSlozka, string strVychoziSlozka, string strStylId, Package package, PackagePart pkgpStyl,
            string strTypJevu,
            string strTypRelace)
        {
            string strSlova = strTypJevu + strPripona;
            string strSlovaTmp = tmpSlozka + strSlova;


            Uri partUriSlova = PackUriHelper.CreatePartUri(new Uri(strVychoziSlozka + strSlova, UriKind.Relative));
            PackagePart pkgpSlova = package.CreatePart(partUriSlova,
                fusFormat == FormatUlozeniSeznamu.Xml ? MediaTypeNames.Text.Xml : MediaTypeNames.Text.Plain,
                CompressionOption.Maximum);
            using (FileStream fileStream = new FileStream(strSlovaTmp, FileMode.Open, FileAccess.Read))
            {
                CopyStream(fileStream, pkgpSlova.GetStream());
            }

            File.Delete(strSlovaTmp);
            PackageRelationship rel;
            if (pkgpStyl == null)
                rel = package.CreateRelationship(pkgpSlova.Uri, TargetMode.Internal, strTypRelace);
            else
                rel = pkgpStyl.CreateRelationship(pkgpSlova.Uri, TargetMode.Internal, strTypRelace);
            return rel;
        }

        private static PackageRelationship UlozitJevyDoBalickuJakoObsah(FormatUlozeniSeznamu fusFormat,
            string strPripona,
            string strJevyTmp, string strVychoziSlozka, string strTypStylu,
            string strStylId, Package package, PackagePart pkgpStyl, Jevy jvJevy, string strTypJevu,
            string strTypRelace)
        {
            string strSlova = strTypJevu + strPripona;
            string strSlovaTmp = strJevyTmp + strSlova;
            JevyService.UlozDoSouboru(jvJevy, strSlovaTmp,
                fusFormat == FormatUlozeniSeznamu.Xml ? FormatUlozeni.Xml : FormatUlozeni.Text);

            Uri partUriSlova = PackUriHelper.CreatePartUri(new Uri(
                strVychoziSlozka + strTypStylu + "\\" + strStylId + "\\" + AdresarovaStruktura.Obsah + "\\" + strSlova,
                UriKind.Relative));
            PackagePart pkgpSlova = package.CreatePart(partUriSlova,
                fusFormat == FormatUlozeniSeznamu.Xml ? MediaTypeNames.Text.Xml : MediaTypeNames.Text.Plain,
                CompressionOption.Maximum);
            using (FileStream fileStream = new FileStream(strSlovaTmp, FileMode.Open, FileAccess.Read))
            {
                CopyStream(fileStream, pkgpSlova.GetStream());
            }

            File.Delete(strSlovaTmp);
            PackageRelationship rel = pkgpStyl.CreateRelationship(pkgpSlova.Uri, TargetMode.Internal, strTypRelace);
            return rel;
        }


        private static VlastnostiBalicku VytvoritVlastnostiBalicek()
        {
            VlastnostiBalicku vlbVlastnosti = new VlastnostiBalicku();

            vlbVlastnosti.Category = "Statistiky";
            vlbVlastnosti.ContentStatus = "Vytvořeno";
            vlbVlastnosti.Created = DateTime.Now;
            vlbVlastnosti.Creator = "Statistik";
            vlbVlastnosti.Description = "Přehled textu ve wordovském dokumentu";
            vlbVlastnosti.Identifier = Guid.NewGuid().ToString();
            vlbVlastnosti.Keywords = "statistiky; přehled; word; dokument";
            vlbVlastnosti.Language = "cs-cz";
            vlbVlastnosti.Revision = "1.0";
            vlbVlastnosti.Subject = "";
            vlbVlastnosti.Title = "";

            vlbVlastnosti.ContentType = JmenneProstory.Statistiky;
            return vlbVlastnosti;
        }

        private static void PriraditVlastnostiBalicku(PackageProperties vlbVlastnosti, Package package)
        {
            package.PackageProperties.Category = vlbVlastnosti.Category;
            package.PackageProperties.ContentStatus = vlbVlastnosti.ContentStatus;
            package.PackageProperties.ContentType = vlbVlastnosti.ContentType;
            package.PackageProperties.Created = vlbVlastnosti.Created;
            package.PackageProperties.Creator = vlbVlastnosti.Creator;
            package.PackageProperties.Description = vlbVlastnosti.Description;
            package.PackageProperties.Identifier = vlbVlastnosti.Identifier;
            package.PackageProperties.Keywords = vlbVlastnosti.Keywords;
            package.PackageProperties.Language = vlbVlastnosti.Language;
            package.PackageProperties.LastModifiedBy = vlbVlastnosti.LastModifiedBy;
            package.PackageProperties.LastPrinted = vlbVlastnosti.LastPrinted;
            package.PackageProperties.Modified = vlbVlastnosti.Modified;
            package.PackageProperties.Revision = vlbVlastnosti.Revision;
            package.PackageProperties.Subject = vlbVlastnosti.Subject;
            package.PackageProperties.Title = vlbVlastnosti.Title;
            package.PackageProperties.Version = vlbVlastnosti.Version;
        }


        public void UlozStatistiky()
        {
            using (Package package = Package.Open(VystupniSoubor, FileMode.Create))
            {
                JevyService jvsJevyService = new JevyService();

                package.PackageProperties.Category = "Statistiky";
                package.PackageProperties.ContentStatus = "Vytvořeno";
                package.PackageProperties.Created = DateTime.Now;
                package.PackageProperties.Creator = "Statistik";
                package.PackageProperties.Description = "Přehled textu ve wordovském dokumentu";
                package.PackageProperties.Identifier = Guid.NewGuid().ToString();
                package.PackageProperties.Keywords = "statistiky; přehled; word; dokument";
                package.PackageProperties.Language = "cs-cz";
                package.PackageProperties.Revision = "1.0";
                package.PackageProperties.Subject = "";
                package.PackageProperties.Title = "";

                string strTemp = Path.GetTempPath();

                Jevy jvVsechnaSlova = null;
                Jevy jvVsechnyZnaky = null;
                SkupinaJevu skjZnaky = null;
                SkupinaJevu skjSlova = null;

                if (SloucitDetaily)
                {
                    //jvVsechnaSlova = new Jevy(Typ.Slova);
                    //jvVsechnyZnaky = new Jevy(Typ.Znaky);
                    skjZnaky = new SkupinaJevu();
                    skjSlova = new SkupinaJevu();
                }


                foreach (Jevy kvp in SkupinaJevu)
                {
                    string sJazyk = kvp.Jazyk;
                    string sIdZnaky = Jevy.GetID(sJazyk, "_znaky");
                    string sIdSlova = Jevy.GetID(sJazyk, "_slova");

                    if (SloucitDetaily)
                    {
                        if (skjSlova != null)
                            if (skjSlova.ContainsID(sIdSlova))
                            {
                                jvVsechnaSlova = skjSlova[sIdSlova];
                            }
                            else
                            {
                                jvVsechnaSlova = new Jevy(TypJevu.Slova, kvp.Zdroj.CelaCesta, sJazyk, "_slova");
                                jvVsechnaSlova.Popis = "Všechna slova v jazyce " + sJazyk;
                                skjSlova.Add(jvVsechnaSlova);
                            }

                        if (skjZnaky != null)
                            if (skjZnaky.ContainsID(sIdZnaky))
                            {
                                jvVsechnyZnaky = skjZnaky[sIdZnaky];
                            }
                            else
                            {
                                jvVsechnyZnaky = new Jevy(TypJevu.Znaky, kvp.Zdroj.CelaCesta, sJazyk, "_znaky");
                                jvVsechnyZnaky.Popis = "Všechny znaky v jazyce " + sJazyk;
                                skjZnaky.Add(jvVsechnyZnaky);
                            }
                    }

                    string strJevyXml = kvp.ID + ".xml";
                    string strSouborJevyXml = strTemp + strJevyXml;

                    string strZnakyXml = kvp.ID + "_znaky" + ".xml";
                    string strSouborZnakyXml = strTemp + strZnakyXml;

                    string strSlovaXml = kvp.ID + "_slova" + ".xml";
                    string strSouborSlovaXml = strTemp + strSlovaXml;

                    jvsJevyService.UlozDoSouboru(kvp, strSouborJevyXml);
                    Jevy jvSlova = new Jevy(TypJevu.Slova, kvp.Zdroj);
                    Jevy jvZnaky = new Jevy(TypJevu.Znaky, kvp.Zdroj);
                    foreach (Jev jv in kvp)
                    {
                        string sText = jv.Nazev;
                        string[] asSlova = Slova.RozdelitTextNaSlova(sText, OdstranitTecku);
                        foreach (string sSlovo in asSlova)
                        {
                            Jev j = new Jev(jv.Jazyk, sSlovo, null, Slovo.Retrograd(sSlovo), jv.Pocet);
                            jvSlova.Add(j);
                            if (jvVsechnaSlova != null)
                                jvVsechnaSlova.Add(j);
                        }

                        foreach (char ch in jv.Nazev)
                        {
                            Jev j = new Jev(jv.Jazyk, ch.ToString(), ch, jv.Pocet);
                            jvZnaky.Add(j);
                            if (jvVsechnyZnaky != null)
                                jvVsechnyZnaky.Add(j);
                        }
                    }

                    jvsJevyService.UlozDoSouboru(jvSlova, strSouborSlovaXml);
                    jvsJevyService.UlozDoSouboru(jvZnaky, strSouborZnakyXml);

                    Uri partUriJevy = PackUriHelper.CreatePartUri(new Uri("Obsah\\" + strJevyXml, UriKind.Relative));
                    PackagePart packagePartDocument =
                        package.CreatePart(partUriJevy, MediaTypeNames.Text.Xml, CompressionOption.Maximum);
                    using (FileStream fileStream = new FileStream(strSouborJevyXml, FileMode.Open, FileAccess.Read))
                    {
                        CopyStream(fileStream, packagePartDocument.GetStream());
                    } // end:using(fileStream) - Close and dispose fileStream.

                    File.Delete(strSouborJevyXml);
                    package.CreateRelationship(packagePartDocument.Uri, TargetMode.Internal, CsTypRelaceJevy);

                    UlozitSlovaNeboZnaky(package, strSlovaXml, strSouborSlovaXml, CsTypRelaceSlova);

                    UlozitSlovaNeboZnaky(package, strZnakyXml, strSouborZnakyXml, CsTypRelaceZnaky);
                    //if (mblnSloucitDetaily)
                    //{
                    //   skjSlova[sIDSlova] = jvVsechnaSlova;
                    //   skjZnaky[sIDZnaky] = jvVsechnyZnaky;
                    //}
                }

                if (SloucitDetaily)
                {
                    if (skjSlova != null)
                        foreach (Jevy jvs in skjSlova)
                        {
                            string strSlovaXml = jvs.ID + ".xml";
                            string strSouborSlovaXml = strTemp + strSlovaXml;
                            jvsJevyService.UlozDoSouboru(jvs, strSouborSlovaXml);
                            UlozitSlovaNeboZnaky(package, strSlovaXml, strSouborSlovaXml, CsTypRelaceSlova);
                        }

                    if (skjZnaky != null)
                        foreach (Jevy jvs in skjZnaky)
                        {
                            string strZnakyXml = jvs.ID + ".xml";
                            string strSouborZnakyXml = strTemp + strZnakyXml;
                            jvsJevyService.UlozDoSouboru(jvs, strSouborZnakyXml);
                            UlozitSlovaNeboZnaky(package, strZnakyXml, strSouborZnakyXml, CsTypRelaceZnaky);
                        }
                }
            }
        }

        private static void UlozitSlovaNeboZnaky(Package package, string strOpcNazevSouboru,
            string strTempSouborJevuXml, string csTypRelace)
        {
            Uri partUriZnaky = PackUriHelper.CreatePartUri(new Uri("Obsah\\" + strOpcNazevSouboru, UriKind.Relative));
            PackagePart packagePartZnaky =
                package.CreatePart(partUriZnaky, MediaTypeNames.Text.Xml, CompressionOption.Maximum);
            using (FileStream fileStream = new FileStream(strTempSouborJevuXml, FileMode.Open, FileAccess.Read))
            {
                CopyStream(fileStream, packagePartZnaky.GetStream());
            } // end:using(fileStream) - Close and dispose fileStream.

            File.Delete(strTempSouborJevuXml);
            package.CreateRelationship(packagePartZnaky.Uri, TargetMode.Internal, csTypRelace);
        }

        //  --------------------------- ExtractPart ---------------------------

        private static void ExtractPart(PackageRelationship docPackageRelationship, Package wdPackage,
            string outputDirectory, string outputFilePath)
        {
            Uri ur = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative), docPackageRelationship.TargetUri);
            PackagePart prt = wdPackage.GetPart(ur);
            FileInfo fi = new FileInfo(outputFilePath);
            string filePath = ExtractPart(prt, outputDirectory);
            // Create the necessary Directories based on the full file path.
            Directory.CreateDirectory(Path.GetDirectoryName(fi.FullName));
            if (fi.Exists && fi.FullName.ToLower() != filePath)
                fi.Delete();
            File.Move(filePath, fi.FullName);
        }

        /// <summary>
        ///   Extracts a specified package part to a target folder.</summary>
        /// <param name="packagePart">
        ///   The package part to extract.</param>
        /// <param name="targetDirectory">
        ///   The path to the target directory folder.</param>
        private static string ExtractPart(
            PackagePart packagePart, string targetDirectory)
        {
            // Create the full path and filename by appending the target
            // directory together with the partUri with its leading slash
            // removed and forward-slashes converted to backward-slashes.
            string filepath = Path.Combine(targetDirectory,
                packagePart.Uri.ToString().TrimStart('/').Replace('/', '\\'));

            // Create the necessary Directories based on the full file path.
            Directory.CreateDirectory(Path.GetDirectoryName(filepath));

            // Write the file from the part's content stream.
            using (FileStream fileStream =
                new FileStream(filepath, FileMode.Create))
            {
                CopyStream(packagePart.GetStream(), fileStream);
            } // end:using(FileStream fileStream) - Close & dispose fileStream.

            return filepath;
        } // end:ExtractPart()


        //  --------------------------- CopyStream ---------------------------
        /// <summary>
        ///   Copies data from a source stream to a target stream.</summary>
        /// <param name="source">
        ///   The source stream to copy from.</param>
        /// <param name="target">
        ///   The destination stream to copy to.</param>
        private static void CopyStream(Stream source, Stream target)
        {
            const int bufSize = 0x1000;
            byte[] buf = new byte[bufSize];
            int bytesRead;
            while ((bytesRead = source.Read(buf, 0, bufSize)) > 0)
                target.Write(buf, 0, bytesRead);
        } // end:CopyStream()
    }
}