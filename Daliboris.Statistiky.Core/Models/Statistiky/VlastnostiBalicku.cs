using System;
using System.IO.Packaging;

namespace Daliboris.Statistiky
{
    public class VlastnostiBalicku : PackageProperties
    {
        /// <summary>
        /// When overridden in a derived class, gets or sets the name given to the <see cref="T:System.IO.Packaging.Package"/> and its content.
        /// </summary>
        /// <returns>
        /// The name given to the <see cref="T:System.IO.Packaging.Package"/> and its content.
        /// </returns>
        public override string Title { get; set; }

        /// <summary>
        /// When overridden in a derived class, gets or sets the topic of the <see cref="T:System.IO.Packaging.Package"/> content.
        /// </summary>
        /// <returns>
        /// The topic of the <see cref="T:System.IO.Packaging.Package"/> content.
        /// </returns>
        public override string Subject { get; set; }

        /// <summary>
        /// When overridden in a derived class, gets or sets a value that identifies the individual or entity that created the <see cref="T:System.IO.Packaging.Package"/> and its content.
        /// </summary>
        /// <returns>
        /// The individual or entity that created the <see cref="T:System.IO.Packaging.Package"/> and its content.
        /// </returns>
        public override string Creator { get; set; }

        /// <summary>
        /// When overridden in a derived class, gets or sets a value that define a delimited set of keywords to support searching and indexing the <see cref="T:System.IO.Packaging.Package"/> and its content.
        /// </summary>
        /// <returns>
        /// A delimited set of keywords to support searching and indexing the <see cref="T:System.IO.Packaging.Package"/> and content.
        /// </returns>
        public override string Keywords { get; set; }

        /// <summary>
        /// When overridden in a derived class, gets or sets a description of the content contained in the <see cref="T:System.IO.Packaging.Package"/>.
        /// </summary>
        /// <returns>
        /// A description of the content contained in the <see cref="T:System.IO.Packaging.Package"/>.
        /// </returns>
        public override string Description { get; set; }

        /// <summary>
        /// When overridden in a derived class, gets or sets a value that identifies the user who last modified the <see cref="T:System.IO.Packaging.Package"/> content.
        /// </summary>
        /// <returns>
        /// The user who last modified the <see cref="T:System.IO.Packaging.Package"/> content.
        /// </returns>
        public override string LastModifiedBy { get; set; }

        /// <summary>
        /// When overridden in a derived class, gets or sets the revision number of the <see cref="T:System.IO.Packaging.Package"/>.
        /// </summary>
        /// <returns>
        /// The revision number of the <see cref="T:System.IO.Packaging.Package"/>.
        /// </returns>
        public override string Revision { get; set; }

        /// <summary>
        /// When overridden in a derived class, gets or sets the date and time the <see cref="T:System.IO.Packaging.Package"/> content was last printed.
        /// </summary>
        /// <returns>
        /// The date and time the <see cref="T:System.IO.Packaging.Package"/> content was last printed.
        /// </returns>
        public override DateTime? LastPrinted { get; set; }

        /// <summary>
        /// When overridden in a derived class, gets or sets the date and time the <see cref="T:System.IO.Packaging.Package"/> was created.
        /// </summary>
        /// <returns>
        /// The date and time the <see cref="T:System.IO.Packaging.Package"/> was initially created.
        /// </returns>
        public override DateTime? Created { get; set; }

        /// <summary>
        /// When overridden in a derived class, gets or sets the date and time the <see cref="T:System.IO.Packaging.Package"/> was last changed.
        /// </summary>
        /// <returns>
        /// The date and time the <see cref="T:System.IO.Packaging.Package"/> was last changed.
        /// </returns>
        public override DateTime? Modified { get; set; }

        /// <summary>
        /// When overridden in a derived class, gets or sets the category of the <see cref="T:System.IO.Packaging.Package"/>.
        /// </summary>
        /// <returns>
        /// The category of the content that is contained in the <see cref="T:System.IO.Packaging.Package"/>.
        /// </returns>
        public override string Category { get; set; }

        /// <summary>
        /// When overridden in a derived class, gets or sets a value that unambiguously identifies the <see cref="T:System.IO.Packaging.Package"/> and its content.
        /// </summary>
        /// <returns>
        /// A value that unambiguously identifies the <see cref="T:System.IO.Packaging.Package"/> and its content.
        /// </returns>
        public override string Identifier { get; set; }

        /// <summary>
        /// When overridden in a derived class, gets or sets a value that represents the type of content that is contained in the <see cref="T:System.IO.Packaging.Package"/>.
        /// </summary>
        /// <returns>
        /// The type of content that is contained in the <see cref="T:System.IO.Packaging.Package"/>.
        /// </returns>
        public override string ContentType { get; set; }

        /// <summary>
        /// When overridden in a derived class, gets or sets a value that identifies the language of the <see cref="T:System.IO.Packaging.Package"/> content.
        /// </summary>
        /// <returns>
        /// A value that identifies the <see cref="T:System.IO.Packaging.Package"/> content language.
        /// </returns>
        public override string Language { get; set; }

        /// <summary>
        /// When overridden in a derived class, gets or sets the version number of the <see cref="T:System.IO.Packaging.Package"/>.
        /// </summary>
        /// <returns>
        /// The version number of the <see cref="T:System.IO.Packaging.Package"/>.
        /// </returns>
        public override string Version { get; set; }

        /// <summary>
        /// When overridden in a derived class, gets or sets a value that represents the status of the <see cref="T:System.IO.Packaging.Package"/>.
        /// </summary>
        /// <returns>
        /// The status of the <see cref="T:System.IO.Packaging.Package"/> content.
        /// </returns>
        public override string ContentStatus { get; set; }
    }
}