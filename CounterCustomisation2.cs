using System;
using System.Threading.Tasks;

namespace ReceiptPrint
{
    /* This class prints each item on single slips and qty > 1 then also different slip*/
    class CounterCustomisation2 : CounterUtilities
    {
        public CounterCustomisation2(dynamic currobj ) : base((object)currobj)
        { }


        public void finalPrint()
        {
            foreach (var itm in this.counterobj.items)
            {

                for (int i = 0; i < (int)itm.qty; i++)
                {
                    //Set Printer
                    this.printing = setPrinter();

                    //Generating final document for the particular type of the counter
                    this.printing.PrintPage += (sender, args) => this.CounterDocumentPrintPage(args, itm);

                    //Printing the actual Receipt
                    this.printing.Print();
                }
            }

        }
    }
}
