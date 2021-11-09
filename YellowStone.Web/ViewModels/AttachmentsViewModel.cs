using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YellowStone.Web.ViewModels
{

    public class AttachmentsViewModel : BaseViewModel
    {
     public   List<AttachmentItems> AttachmentItems { get; set; }

        public AttachmentsViewModel(bool isSuccessful)
        {
            IsSuccessful = isSuccessful;
        }
    }

    public class AttachmentItems
    {
        public long Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string DateUploaded { get; set; }
        public string RequestId { get; set; }
    }
}
