
namespace Services.EmailServiceProvider.Interface
{
    /// <summary>
    /// A data structure to store the file name and its content to send as an attachment of an email message.
    /// </summary>
    public class FileAttachment
    {
        /// <summary>
        /// A <see cref="string"/> that contains the name of the attached file.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A <see cref="string"/> that contains the <b>Base 64</b> encoded representation of the actual content of the file that is being attached.
        /// </summary>
        public string Content { get; set; }
    }
}
