using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace BarcodeGenerator
{

    public  class GenerateBarcode
    {
        Barcode bCode = new Barcode();
        public void CreateImage(string path, List<string> barcodeTextList, TYPE type, int width, int height,AlignmentPositions alm,bool isLabelShow=true)
        {
            try
            {
                bCode.Alignment = alm;
                bCode.IncludeLabel = isLabelShow;
                bCode.LabelPosition = LabelPositions.BOTTOMCENTER;
                GetEncodeType(type);
                foreach (var _barCode in barcodeTextList)
                {
                    Image myimg = bCode.Encode(type, _barCode, width, height);
                    string _path = path + _barCode + ".jpg";
                    myimg.Save(_path);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public void SaveImage()
        //{
        //    SaveFileDialog sfd = new SaveFileDialog();
        //    sfd.Filter = "BMP (*.bmp)|*.bmp|GIF (*.gif)|*.gif|JPG (*.jpg)|*.jpg|PNG (*.png)|*.png|TIFF (*.tif)|*.tif";
        //    sfd.FilterIndex = 2;
        //    sfd.AddExtension = true;
        //    if (sfd.ShowDialog() == DialogResult.OK)
        //    {
        //        SaveTypes savetype = SaveTypes.UNSPECIFIED;
        //        switch (sfd.FilterIndex)
        //        {
        //            case 1: /* BMP */  savetype = SaveTypes.BMP; break;
        //            case 2: /* GIF */  savetype = SaveTypes.GIF; break;
        //            case 3: /* JPG */  savetype = SaveTypes.JPG; break;
        //            case 4: /* PNG */  savetype = SaveTypes.PNG; break;
        //            case 5: /* TIFF */ savetype = SaveTypes.TIFF; break;
        //            default: break;
        //        }//switch
        //        bCode.SaveImage(sfd.FileName, savetype);
        //    }//if
        //}

        private  void GetEncodeType(TYPE type)
        {
            switch (type.ToString())
            {
                case "UPC-A": type = TYPE.UPCA; break;
                case "UPC-E": type = TYPE.UPCE; break;
                case "UPC 2 Digit Ext.": type = TYPE.UPC_SUPPLEMENTAL_2DIGIT; break;
                case "UPC 5 Digit Ext.": type = TYPE.UPC_SUPPLEMENTAL_5DIGIT; break;
                case "EAN-13": type = TYPE.EAN13; break;
                case "JAN-13": type = TYPE.JAN13; break;
                case "EAN-8": type = TYPE.EAN8; break;
                case "ITF-14": type = TYPE.ITF14; break;
                case "Codabar": type = TYPE.Codabar; break;
                case "PostNet": type = TYPE.PostNet; break;
                case "Bookland/ISBN": type = TYPE.BOOKLAND; break;
                case "Code 11": type = TYPE.CODE11; break;
                case "Code 39": type = TYPE.CODE39; break;
                case "Code 39 Extended": type = TYPE.CODE39Extended; break;
                case "Code 39 Mod 43": type = TYPE.CODE39_Mod43; break;
                case "Code 93": type = TYPE.CODE93; break;
                case "LOGMARS": type = TYPE.LOGMARS; break;
                case "MSI": type = TYPE.MSI_Mod10; break;
                case "Interleaved 2 of 5": type = TYPE.Interleaved2of5; break;
                case "Standard 2 of 5": type = TYPE.Standard2of5; break;
                case "Code 128": type = TYPE.CODE128; break;
                case "Code 128-A": type = TYPE.CODE128A; break;
                case "Code 128-B": type = TYPE.CODE128B; break;
                case "Code 128-C": type = TYPE.CODE128C; break;
                case "Telepen": type = TYPE.TELEPEN; break;
                case "FIM": type = TYPE.FIM; break;
                case "Pharmacode": type = TYPE.PHARMACODE; break;
            }//switch

        }
    }
}
