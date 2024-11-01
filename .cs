using System;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Configuration;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace ReceiptPrint
{
  class NewSlipDesign
  {
    public dynamic receiptobj;
    public Font dfont, cutFont;
    const String underline = "-------------------------------------------------------------------------------------";
    public NewSlipDesign(dynamic currobj)
    {
      this.receiptobj = currobj;
    }
    public static Bitmap ResizeImage(Image image, int width, int height)
    {
      var destRect = new Rectangle(0, 0, width, height);
      var destImage = new Bitmap(width, height);
      destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
      using (var graphics = Graphics.FromImage(destImage))
      {
        graphics.CompositingMode = CompositingMode.SourceCopy;
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

      using (var wrapMode = new ImageAttributes())
      {
         wrapMode.SetWrapMode(WrapMode.TileFlipXY);
         graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
      }

      }
        return destImage;
    }
      public enum FontSize
      {

        SMALL = 10,
        MEDIUM = 11,
        LARGE = 12
      }

      public void receiptDocumentPrintPage(PrintPageEventArgs e, int pr_width)
      {
        dynamic Receiptdata = this.receiptobj;
        //FONT SIZE
        int fz = (int)FontSize.SMALL;
        int f1 = fz * pr_width / 300;
        Graphics graphic = e.Graphics;
        int startX = 2;
        int startY = 5;
        int offset = 0;
        int Stringlength1 = (int)(0.62 * pr_width), Stringlength2 = (int)(0.53 * pr_width), Stringlength3 = (int)(0.25 * pr_width);//Measure String Size for Rectangle Width
        Rectangle rect;
        SizeF stringSize = new SizeF();
        StringFormat C = new StringFormat();
        C.Alignment = StringAlignment.Center;
        StringFormat L = new StringFormat();
        L.Alignment = StringAlignment.Near;
        StringFormat R = new StringFormat();
        R.Alignment = StringAlignment.Far;

         if (Receiptdata.data != null)
         {
          //FONT SIZE ENUMS
           foreach (var item in Receiptdata.data)
           {
             if (item.fs == "s")
             {
               fz = (int)FontSize.SMALL;
               f1 = fz * pr_width / 300;

             }
             else if (item.fs == "m")
             {
               fz = (int)FontSize.MEDIUM;
                f1 = fz * pr_width / 300;
             }
             else if (item.fs == "l")
             {
               fz = (int)FontSize.LARGE;
               f1 = fz * pr_width / 300;
             }

               // ENUMS FOR FONT STYLE, BOLD, NORMAL and ITALIC
             if (item.ft == "b")
             {
                this.dfont = new Font("Arial", f1, FontStyle.Bold);
                this.cutFont = new Font("Arial", (int)(f1), FontStyle.Strikeout | FontStyle.Bold);
             }
             else if (item.ft == "n")
             {
                this.dfont = new Font("Arial", (int)(f1));
                this.cutFont = new Font("Arial", (int)(f1), FontStyle.Strikeout);
             }
             else if (item.ft == "i")
             {
              this.dfont = new Font("Arial", f1, FontStyle.Italic);
              this.cutFont = new Font("Arial", (int)(f1), FontStyle.Strikeout | FontStyle.Italic);
             }
             if (item.key == "" && item.value is JArray)
             {
               foreach (var data in item.value)
               {
                 var myElem = item.value;
                 JArray items = (JArray)item["value"];
                 int length = items.Count; //Lenght of value of JSON response
                 if (length == 1)
                 {
                    //Print Header,
                      if (item.fa == "c")
                      {
                       stringSize = graphic.MeasureString(data.name.ToString(), dfont, (int)(pr_width));
                       rect = new Rectangle(startX, startY + offset, pr_width, (int)stringSize.Height);
                       graphic.DrawString(data.name.ToString(), dfont, new SolidBrush(Color.Black), rect, C);
                       offset += (int)stringSize.Height + 1;
                      }
                      //Print Order No, Date, time etc...
                  else if (item.fa == "l")
                  {
                    Rectangle rect1;
                    int offsetx = 0;
                    // Font Sfont = new Font("Arial", (int)(f1));
                    stringSize = graphic.MeasureString(data.name.ToString(), dfont, (int)(pr_width));
                    rect1 = new Rectangle(startX + offsetx, startY + offset, (int)(pr_width), (int)stringSize.Height);
                    graphic.DrawString(data.name.ToString(), dfont, Brushes.Black, rect1, L);
                    offset += (int)stringSize.Height + 2;
                  }
                 }

                            //Print QTY , ITEM NAME & AMOUNT 
                 else if (length == 3)
                 {
                     Rectangle rect1, rect2, rect3;
                      // Font Sfont = new Font("Arial", (int)(f1));
                     float Sfontheight = dfont.GetHeight();
                     int offsetx = 0, pd = (int)(0.005 * pr_width);
                     SizeF stringSize2, stringSize3 = new SizeF();
                     stringSize2 = graphic.MeasureString(myElem[1].name.ToString(), dfont, (int)(0.58 * pr_width));
                     stringSize3 = graphic.MeasureString(myElem[2].name.ToString(), dfont, (int)(0.30 * pr_width));

                     rect1 = new Rectangle(startX + offsetx, startY + offset, (int)(0.2 * pr_width) + pd, (int)Sfontheight);
                     graphic.DrawString(myElem[0].name.ToString(), dfont, Brushes.Black, rect1, L);
                     offsetx += (int)(0.15 * pr_width) + pd;

                     rect2 = new Rectangle(startX + offsetx, startY + offset, (int)(0.58 * pr_width), (int)stringSize2.Height);
                     graphic.DrawString(myElem[1].name.ToString(), dfont, Brushes.Black, rect2, L);
                     offsetx += (int)(0.52 * pr_width) + pd;

                     rect3 = new Rectangle(startX + offsetx, startY + offset, (int)(0.30 * pr_width), (int)stringSize3.Height);
                     graphic.DrawString(myElem[2].name.ToString(), dfont, Brushes.Black, rect3, R);
                     offset += (int)stringSize2.Height + 3;

                     break;

                 }

                 // Print Void Items 
                 else if (length == 2 && myElem[0].strike == 1)
                 {                              
                      Rectangle rect1, rect3;
                      SizeF stringSize1 = new SizeF();
                      int offsetx = 0, pd = (int)(0.005 * pr_width);
                      stringSize1 = graphic.MeasureString(myElem[0].name.ToString(), dfont, (int)(0.7 * pr_width));
                      stringSize = graphic.MeasureString(myElem[1].name.ToString(), dfont, (int)(0.66 * pr_width));
                     // (int)Sfontheight + 2;
                      rect1 = new Rectangle(startX + offsetx, startY + offset, (int)(0.7 * pr_width), (int)stringSize1.Height);
                      graphic.DrawString(myElem[0].name.ToString(), cutFont, Brushes.Black, rect1, L);
                      offsetx += (int)(0.32 * pr_width + pd);

                      rect3 = new Rectangle(startX + offsetx, startY + offset, (int)(0.66 * pr_width), (int)stringSize.Height);
                      graphic.DrawString(myElem[1].name.ToString(), cutFont, Brushes.Black, rect3, R);
                      offset += (int)stringSize1.Height + 3;
                      break;
                              
                 }
                            //Print Subtotal ------- Amount:  
                 else if (length == 2)
                 {
                      Rectangle rect1, rect3;
                      SizeF stringSize1, stringSize4 = new SizeF();
                      int offsetx = 0, pd = (int)(0.005 * pr_width);
                      int p1 = (int)(0.051 * pr_width);
                      stringSize1 = graphic.MeasureString(myElem[0].name.ToString(), dfont, (int)(0.7 * pr_width));
                      stringSize4 = graphic.MeasureString(myElem[1].name.ToString(), dfont, (int)(0.6 * pr_width));
                       {
                        rect1 = new Rectangle(startX + offsetx, startY + offset, (int)(0.7 * pr_width), (int)stringSize1.Height);
                        graphic.DrawString(myElem[0].name.ToString(), dfont, Brushes.Black, rect1, L);
                        offsetx += (int)(0.37 * pr_width + pd);
                       }
                                
                       {
                        rect3 = new Rectangle(startX + offsetx, startY + offset, (int)(0.6 * pr_width), (int)stringSize4.Height);
                        graphic.DrawString(myElem[1].name.ToString(), dfont, Brushes.Black, rect3, R);
                       }
                       offset += (int)stringSize1.Height-p1;
                       offset += (int)stringSize4.Height ;
                               
                      break;
                 }
               }
             }

                    //Print underline (---------)
                else if (item.key == "_line_" && item.value == "-")
                {
                 Font font = new Font("Arial", f1);
                 float fontheight = font.GetHeight();
                 graphic.DrawString(underline, font, Brushes.Black, new Point(startX, startY + offset));
                 offset += (int)fontheight + 1;
                }
                    //Print Restro Logo
                else if (item.key == "_img_")
                {
                        // Create Image
                  WebClient client = new WebClient();
                  Stream stream = client.OpenRead(item.value.ToString());
                  var img = Image.FromStream(stream);

                  //Resize Image
                  Bitmap bmap = ResizeImage(img, 100, 100);

                  //Draw Image
                  rect = new Rectangle(startX + (pr_width - 100) / 2, startY, 100, 100);
                  graphic.DrawImage(bmap, rect);
                  offset = offset + 110;
                }
                    // Print Footer....
                else if (item.key == "" && item.value is object)
                {
  
                  float Bfontheight = dfont.GetHeight();
                  rect = new Rectangle(startX, startY + offset, pr_width, 0);
                  graphic.DrawString(item.value.ToString(), dfont, new SolidBrush(Color.Black), rect, C);
                  offset += (int)Bfontheight + 1;
                }

           }
         }
      }

        public void finalPrint()
        {
            //Finding Default printer
            PrintDocument pr = new PrintDocument();
            string defaultPrinterName = pr.PrinterSettings.PrinterName;
            int pr_width;

            //Selecting the Required Printer and find the exact width of the page for it
            pr.PrinterSettings.PrinterName = this.receiptobj.printerName;
            if (pr.PrinterSettings.IsValid && this.receiptobj.printerName != null)
            {
                pr_width = pr.DefaultPageSettings.PaperSize.Width;
            }
            else
            {
                pr.PrinterSettings.PrinterName = defaultPrinterName;
                pr_width = 280;
                pr.DefaultPageSettings.PaperSize = new PaperSize("Custom", pr_width, 5 * 2400);

            }

            //Generating final document for the particular type of the counter
            pr.PrintPage += (sender, args) => receiptDocumentPrintPage(args, pr_width);

            //Printing the actual Receipt
            Console.WriteLine(pr_width);
            pr.Print();

        }
    }
}




