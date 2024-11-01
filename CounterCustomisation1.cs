using System;
using System.Threading.Tasks;

namespace ReceiptPrint
{

    /*This Class prints each item on individual slips*/
    class CounterCustomisation1 : CounterUtilities
    {
        public CounterCustomisation1(dynamic currobj) : base((object)currobj)
        { }


        public void finalPrint()
        {
            foreach (var itm in this.counterobj.items)
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
