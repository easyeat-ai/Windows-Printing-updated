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
        const String oneLineSpace = "                                                                                     ";
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

        public void receiptDocumentPrintPage(PrintPageEventArgs e, int p_width)
        {
            dynamic Receiptdata = this.receiptobj;
            //FONT SIZE
            int fz = (int)FontSize.SMALL;
            int f1 = fz * p_width / 300;
            Graphics graphic = e.Graphics;
            int startX = 2;
            int startY = 5;
            int offset = 0;
            int Stringlength1 = (int)(0.62 * p_width), Stringlength2 = (int)(0.53 * p_width), Stringlength3 = (int)(0.25 * p_width);//Measure String Size for Rectangle Width
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
                        f1 = fz * p_width / 300;

                    }
                    else if (item.fs == "m")
                    {
                        fz = (int)FontSize.MEDIUM;
                        f1 = fz * p_width / 300;
                    }
                    else if (item.fs == "l")
                    {
                        fz = (int)FontSize.LARGE;
                        f1 = fz * p_width / 300;
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
                                    stringSize = graphic.MeasureString(data.name.ToString(), dfont, (int)(p_width));
                                    rect = new Rectangle(startX, startY + offset, p_width, (int)stringSize.Height);
                                    graphic.DrawString(data.name.ToString(), dfont, new SolidBrush(Color.Black), rect, C);
                                    offset += (int)stringSize.Height + 1;
                                }
                                //Print Order No, Date, time etc...
                                else if (item.fa == "l")
                                {
                                    Rectangle rect1;
                                    int offsetx = 0;
                                    // Font Sfont = new Font("Arial", (int)(f1));
                                    stringSize = graphic.MeasureString(data.name.ToString(), dfont, (int)(p_width));
                                    rect1 = new Rectangle(startX + offsetx, startY + offset, (int)(p_width), (int)stringSize.Height);
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
                                int offsetx = 0, pd = (int)(0.005 * p_width);
                                SizeF stringSize2, stringSize3 = new SizeF();
                                stringSize2 = graphic.MeasureString(myElem[1].name.ToString(), dfont, (int)(0.58 * p_width));
                                stringSize3 = graphic.MeasureString(myElem[2].name.ToString(), dfont, (int)(0.48 * p_width));

                                rect1 = new Rectangle(startX + offsetx, startY + offset, (int)(0.2 * p_width) + pd, (int)Sfontheight);
                                graphic.DrawString(myElem[0].name.ToString(), dfont, Brushes.Black, rect1, L);
                                offsetx += (int)(0.12 * p_width) + pd;

                                rect2 = new Rectangle(startX + offsetx, startY + offset, (int)(0.53 * p_width), (int)stringSize2.Height);
                                graphic.DrawString(myElem[1].name.ToString(), dfont, Brushes.Black, rect2, L);
                                offsetx += (int)(0.49 * p_width) + pd;

                                rect3 = new Rectangle(startX + offsetx, startY + offset, (int)(0.37 * p_width), (int)stringSize3.Height);
                                graphic.DrawString(myElem[2].name.ToString(), dfont, Brushes.Black, rect3, R);
                                offset += (int)stringSize2.Height + 3;

                                break;

                            }

                            // Print Void Items 
                            else if (length == 2 && myElem[0].strike == 1)
                            {
                                Rectangle rect1, rect3;
                                SizeF stringSize1 = new SizeF();
                                int offsetx = 0, pd = (int)(0.005 * p_width);
                                stringSize1 = graphic.MeasureString(myElem[0].name.ToString(), dfont, (int)(0.7 * p_width));
                                stringSize = graphic.MeasureString(myElem[1].name.ToString(), dfont, (int)(0.66 * p_width));
                                // (int)Sfontheight + 2;
                                rect1 = new Rectangle(startX + offsetx, startY + offset, (int)(0.7 * p_width), (int)stringSize1.Height);
                                graphic.DrawString(myElem[0].name.ToString(), cutFont, Brushes.Black, rect1, L);
                                offsetx += (int)(0.32 * p_width + pd);

                                rect3 = new Rectangle(startX + offsetx, startY + offset, (int)(0.66 * p_width), (int)stringSize.Height);
                                graphic.DrawString(myElem[1].name.ToString(), cutFont, Brushes.Black, rect3, R);
                                offset += (int)stringSize1.Height + 3;
                                break;

                            }
                            //Print Subtotal ------- Amount:  
                            else if (length == 2)
                                
                            {
                                    Rectangle rect1, rect3;
                                    Font font = new Font("Arial", f1);
                                    SizeF stringSize1, stringSize4 = new SizeF();
                                    int offsetx = 0, pd = (int)(0.005 * p_width);
                                    stringSize1 = graphic.MeasureString(myElem[0].name.ToString(), dfont, (int)(0.7 * p_width));
                                    stringSize4 = graphic.MeasureString(myElem[1].name.ToString(), dfont, (int)(0.57 * p_width));
                                    rect1 = new Rectangle(startX + offsetx, startY + offset, (int)(0.6 * p_width), (int)stringSize1.Height);
                                    graphic.DrawString(myElem[0].name.ToString(), dfont, Brushes.Black, rect1, L);
                                    offsetx += (int)(0.41 * p_width + pd);

                                    rect3 = new Rectangle(startX + offsetx, startY + offset, (int)(0.57 * p_width), (int)stringSize4.Height);
                                    graphic.DrawString(myElem[1].name.ToString(), dfont, Brushes.Black, rect3, R);
                                    offset += (int)stringSize1.Height;

                                    if ((myElem[0].name.ToString().Contains("PAYMENT MODE") && (int)stringSize1.Height < (int)stringSize4.Height))
                                    {
                                        // Case of payment mode values taking more than one line
                                        int noOfLinesRequired = (int)Math.Ceiling(stringSize4.Height / stringSize1.Height) - 1;
                                        Console.WriteLine($"noOfLinesRequired: {noOfLinesRequired}");
                                        while (noOfLinesRequired > 0)
                                        {
                                            float fontheight = font.GetHeight();
                                            graphic.DrawString(oneLineSpace, font, Brushes.Black, new Point(startX, startY + offset));
                                            offset += (int)fontheight + 1;
                                            noOfLinesRequired--; // Decrement the counter
                                        }
                                    }
                                
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
                        rect = new Rectangle(startX + (p_width - 100) / 2, startY, 100, 100);
                        graphic.DrawImage(bmap, rect);
                        offset = offset + 110;
                    }
                    // Print Footer....
                    else if (item.key == "" && item.value is object)
                    {

                        float Bfontheight = dfont.GetHeight();
                        rect = new Rectangle(startX, startY + offset, p_width, 0);
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
            int p_width;

            //Selecting the Required Printer and find the exact width of the page for it
            pr.PrinterSettings.PrinterName = this.receiptobj.ptr_name;
            if (pr.PrinterSettings.IsValid && this.receiptobj.ptr_name != null)
            {
                p_width = pr.DefaultPageSettings.PaperSize.Width;
            }
            else
            {
                pr.PrinterSettings.PrinterName = defaultPrinterName;
                p_width = 280;
                pr.DefaultPageSettings.PaperSize = new PaperSize("Custom", p_width, 5 * 2400);

            }

            //Generating final document for the particular type of the counter
            pr.PrintPage += (sender, args) => receiptDocumentPrintPage(args, p_width);

            //Printing the actual Receipt
            Console.WriteLine(p_width);
            pr.Print();

        }
    }
}



