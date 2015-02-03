// --------------------------------------------------------------------------
// Licensed under MIT License.
//
// Outlook DnD Data Reader
// 
// File     : OleDataReader.cs
// Author   : Tobias Viehweger <tobias.viehweger@yasoon.com / @mnkypete>
//
// -------------------------------------------------------------------------- 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FritzBoxDial
{
    public partial class MyDndForm : Form
    {
       
        public MyDndForm()
        {
            InitializeComponent();
        } 
        
        private LocalDropTarget myDropTarget = null;

        private void MyDnDForm_Shown(object sender, EventArgs e)
        {
            var res = RegisterDragDrop(this.DnDPanel.Handle, GetDropTarget());
        }

        public IOleDropTarget GetDropTarget()
        {
            this.myDropTarget = new LocalDropTarget();
            return myDropTarget;
        }

        //// Declare the delegate (if using non-generic pattern).
        //public delegate void DnDEventHandler(object sender, DnDEventArgs e);

        //// Declare the event.
        //public event DnDEventHandler DnDEvent;
            
        //// Wrap the event in a protected virtual method
        //// to enable derived classes to raise the event.
        //public virtual void RaiseDnDEvent(MyOleOutlookData d)
        //{
        //    // Raise the event by using the () operator.
        //    if (DnDEvent != null)
        //        DnDEvent(this, new DnDEventArgs(d));
        //}

        public class LocalDropTarget : IOleDropTarget
        {
            public MyOleOutlookData OnDragDrop(System.Windows.DataObject d)
            {
                var formats = d.GetFormats();

                MyOleOutlookData retdata = new MyOleOutlookData();

                foreach (var format in formats)
                    Trace.WriteLine(format);

                var data = d.GetData("RenPrivateMessages");

                if (data is MemoryStream)
                {
                    BinaryReader reader = new BinaryReader(data as MemoryStream);
                    // int folderIdLength = reader.ReadInt32();

                    //1. First 4 bytes are the length of the FolderId (In bytes)
                    // Note: These are possibly uint? We don't expect it to be that long nevertheless..
                    int folderIdLength = reader.ReadInt32();

                    //2. Read FolderId
                    byte[] folderId = reader.ReadBytes(folderIdLength);
                    string folderIdHex = ByteArrayToString(folderId);

                    //3. Next 4 bytes are the StoreId length (In bytes)
                    int storeIdLength = reader.ReadInt32();

                    //4. Read StoreId
                    byte[] storeId = reader.ReadBytes(storeIdLength);
                    string storeIdHex = ByteArrayToString(storeId);

                    //5. There are now some bytes which are not identified yet..
                    reader.ReadBytes(4);
                    reader.ReadBytes(4);
                    reader.ReadBytes(4); // <== These appear to be folder dependent somehow..

                    //6. Read items count, again, we assume int instead of uint because that much items
                    //   => Other problems =)
                    int itemCount = reader.ReadInt32();

                    MyOleOutlookItemData[] items = new MyOleOutlookItemData[itemCount];

                    for (int i = 0; i < itemCount; i++)
                    {
                        //First 4 bytes, represent the MAPI property 0x8014 ("SideEffects" in OlSpy)
                        int sideEffects = reader.ReadInt32();

                        //Next byte tells us the length of the message class string (i.e. IPM.Note)
                        byte classLength = reader.ReadByte();

                        //Now, read type
                        string messageClass = Encoding.ASCII.GetString(reader.ReadBytes(classLength));

                        //Next, read the unicode char (!) count of the subject 
                        // Note: It seems that Outlook limits this to 255, cross reference mail spec sometime..
                        byte subjectLength = reader.ReadByte();

                        //Read the subject, note that this is unicode, so we need to read 2 bytes per char!
                        string subject = Encoding.Unicode.GetString(reader.ReadBytes(subjectLength * 2));

                        //Next up: EntryID including it's length (same as for store + folder)
                        int entryIdLength = reader.ReadInt32();
                        byte[] entryId = reader.ReadBytes(entryIdLength);
                        string entryIdHex = ByteArrayToString(entryId);

                        //Now the SearchKey MAPI property of the item
                        int searchKeyLength = reader.ReadInt32();
                        byte[] searchKey = reader.ReadBytes(searchKeyLength);
                        string searchKeyHex = ByteArrayToString(searchKey);

                        //Some more stuff which is not quite clear, the next 4 bytes seem to be always
                        // => E0 80 E9 5A
                        reader.ReadBytes(4);

                        //The next 24 byte are some more flags which are not worked out yet, afterwards
                        // the next item begins
                        reader.ReadBytes(24);

                        items[i] = new MyOleOutlookItemData
                        {
                            EntryId = entryIdHex,
                            MessageClass = messageClass,
                            SearchKey = searchKeyHex,
                            Subject = subject
                        };
                    }
                    
                    retdata.StoreId = storeIdHex;
                    retdata.FolderId = folderIdHex;
                    retdata.Items = items;
                } 
                return retdata;
            }

            public int OleDragEnter(object pDataObj, int grfKeyState, long pt, ref int pdwEffect)
            {
                Marshal.FinalReleaseComObject(pDataObj);
                return 0;
            }

            public int OleDragOver(int grfKeyState, long pt, ref int pdwEffect)
            {
                return 0;
            }

            public int OleDragLeave()
            {
                return 0;
            }

            public int OleDrop(object pDataObj, int grfKeyState, long pt, ref int pdwEffect)
            {
                System.Windows.DataObject data = new System.Windows.DataObject(pDataObj);
                OnDragDrop(data);
                Marshal.FinalReleaseComObject(pDataObj);
                return 0;
            }

            private string ByteArrayToString(byte[] ba)
            {
                StringBuilder hex = new StringBuilder(ba.Length * 2);
                foreach (byte b in ba)
                    hex.AppendFormat("{0:x2}", b);
                return hex.ToString();
            }
        }

        //Native imports
        [DllImport("ole32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int RegisterDragDrop(IntPtr hwnd, IOleDropTarget target);

        [ComImport(), Guid("00000122-0000-0000-C000-000000000046"), InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleDropTarget
        {
            [PreserveSig]
            int OleDragEnter(
                [In, MarshalAs(UnmanagedType.Interface)]
                object pDataObj,
                [In, MarshalAs(UnmanagedType.U4)]
                int grfKeyState,
                [In, MarshalAs(UnmanagedType.U8)]
                long pt,
                [In, Out]
                ref int pdwEffect);

            [PreserveSig]
            int OleDragOver(
                [In, MarshalAs(UnmanagedType.U4)]
                int grfKeyState,
                [In, MarshalAs(UnmanagedType.U8)]
                long pt,
                [In, Out]
                ref int pdwEffect);

            [PreserveSig]
            int OleDragLeave();

            [PreserveSig]
            int OleDrop(
                [In, MarshalAs(UnmanagedType.Interface)]
                object pDataObj,
                [In, MarshalAs(UnmanagedType.U4)]
                int grfKeyState,
                [In, MarshalAs(UnmanagedType.U8)]
                long pt,
                [In, Out]
                ref int pdwEffect);
        }
    }

    //public class DnDEventArgs
    //{
    //    public DnDEventArgs(MyOleOutlookData d)
    //    {
    //        pushOleOutlookItemData = d;
    //    }
    //    public MyOleOutlookData pushOleOutlookItemData
    //    {// readonly
    //        get;
    //        private set;
    //    }
    //}
}
