﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Warden.Core.Utils
{
    /// <summary>
    /// Reads in the header information of the Portable Executable format.
    /// Provides information such as the date the assembly was compiled.
    /// </summary>
    public class PeHeaderReader
    {
        #region File Header Structures

        public struct ImageDosHeader
        {      // DOS .EXE header
            public ushort EMagic;              // Magic number
            public ushort ECblp;               // Bytes on last page of file
            public ushort ECp;                 // Pages in file
            public ushort ECrlc;               // Relocations
            public ushort ECparhdr;            // Size of header in paragraphs
            public ushort EMinalloc;           // Minimum extra paragraphs needed
            public ushort EMaxalloc;           // Maximum extra paragraphs needed
            public ushort ESs;                 // Initial (relative) SS value
            public ushort ESp;                 // Initial SP value
            public ushort ECsum;               // Checksum
            public ushort EIp;                 // Initial IP value
            public ushort ECs;                 // Initial (relative) CS value
            public ushort ELfarlc;             // File address of relocation table
            public ushort EOvno;               // Overlay number
            public ushort ERes0;              // Reserved words
            public ushort ERes1;              // Reserved words
            public ushort ERes2;              // Reserved words
            public ushort ERes3;              // Reserved words
            public ushort EOemid;              // OEM identifier (for e_oeminfo)
            public ushort EOeminfo;            // OEM information; e_oemid specific
            public ushort ERes20;             // Reserved words
            public ushort ERes21;             // Reserved words
            public ushort ERes22;             // Reserved words
            public ushort ERes23;             // Reserved words
            public ushort ERes24;             // Reserved words
            public ushort ERes25;             // Reserved words
            public ushort ERes26;             // Reserved words
            public ushort ERes27;             // Reserved words
            public ushort ERes28;             // Reserved words
            public ushort ERes29;             // Reserved words
            public uint ELfanew;             // File address of new exe header
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ImageDataDirectory
        {
            public uint VirtualAddress;
            public uint Size;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ImageOptionalHeader32
        {
            public ushort Magic;
            public byte MajorLinkerVersion;
            public byte MinorLinkerVersion;
            public uint SizeOfCode;
            public uint SizeOfInitializedData;
            public uint SizeOfUninitializedData;
            public uint AddressOfEntryPoint;
            public uint BaseOfCode;
            public uint BaseOfData;
            public uint ImageBase;
            public uint SectionAlignment;
            public uint FileAlignment;
            public ushort MajorOperatingSystemVersion;
            public ushort MinorOperatingSystemVersion;
            public ushort MajorImageVersion;
            public ushort MinorImageVersion;
            public ushort MajorSubsystemVersion;
            public ushort MinorSubsystemVersion;
            public uint Win32VersionValue;
            public uint SizeOfImage;
            public uint SizeOfHeaders;
            public uint CheckSum;
            public ushort Subsystem;
            public ushort DllCharacteristics;
            public uint SizeOfStackReserve;
            public uint SizeOfStackCommit;
            public uint SizeOfHeapReserve;
            public uint SizeOfHeapCommit;
            public uint LoaderFlags;
            public uint NumberOfRvaAndSizes;

            public ImageDataDirectory ExportTable;
            public ImageDataDirectory ImportTable;
            public ImageDataDirectory ResourceTable;
            public ImageDataDirectory ExceptionTable;
            public ImageDataDirectory CertificateTable;
            public ImageDataDirectory BaseRelocationTable;
            public ImageDataDirectory Debug;
            public ImageDataDirectory Architecture;
            public ImageDataDirectory GlobalPtr;
            public ImageDataDirectory TLSTable;
            public ImageDataDirectory LoadConfigTable;
            public ImageDataDirectory BoundImport;
            public ImageDataDirectory IAT;
            public ImageDataDirectory DelayImportDescriptor;
            public ImageDataDirectory CLRRuntimeHeader;
            public ImageDataDirectory Reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ImageOptionalHeader64
        {
            public ushort Magic;
            public byte MajorLinkerVersion;
            public byte MinorLinkerVersion;
            public uint SizeOfCode;
            public uint SizeOfInitializedData;
            public uint SizeOfUninitializedData;
            public uint AddressOfEntryPoint;
            public uint BaseOfCode;
            public ulong ImageBase;
            public uint SectionAlignment;
            public uint FileAlignment;
            public ushort MajorOperatingSystemVersion;
            public ushort MinorOperatingSystemVersion;
            public ushort MajorImageVersion;
            public ushort MinorImageVersion;
            public ushort MajorSubsystemVersion;
            public ushort MinorSubsystemVersion;
            public uint Win32VersionValue;
            public uint SizeOfImage;
            public uint SizeOfHeaders;
            public uint CheckSum;
            public ushort Subsystem;
            public ushort DllCharacteristics;
            public ulong SizeOfStackReserve;
            public ulong SizeOfStackCommit;
            public ulong SizeOfHeapReserve;
            public ulong SizeOfHeapCommit;
            public uint LoaderFlags;
            public uint NumberOfRvaAndSizes;

            public ImageDataDirectory ExportTable;
            public ImageDataDirectory ImportTable;
            public ImageDataDirectory ResourceTable;
            public ImageDataDirectory ExceptionTable;
            public ImageDataDirectory CertificateTable;
            public ImageDataDirectory BaseRelocationTable;
            public ImageDataDirectory Debug;
            public ImageDataDirectory Architecture;
            public ImageDataDirectory GlobalPtr;
            public ImageDataDirectory TLSTable;
            public ImageDataDirectory LoadConfigTable;
            public ImageDataDirectory BoundImport;
            public ImageDataDirectory IAT;
            public ImageDataDirectory DelayImportDescriptor;
            public ImageDataDirectory CLRRuntimeHeader;
            public ImageDataDirectory Reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ImageFileHeader
        {
            public ushort Machine;
            public ushort NumberOfSections;
            public uint TimeDateStamp;
            public uint PointerToSymbolTable;
            public uint NumberOfSymbols;
            public ushort SizeOfOptionalHeader;
            public ushort Characteristics;
        }

        // Grabbed the following 2 definitions from http://www.pinvoke.net/default.aspx/Structures/IMAGE_SECTION_HEADER.html

        [StructLayout(LayoutKind.Explicit)]
        public struct ImageSectionHeader
        {
            [FieldOffset(0)]
            [IgnoreDataMember]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public char[] Name;
            [FieldOffset(8)]
            public uint VirtualSize;
            [FieldOffset(12)]
            public uint VirtualAddress;
            [FieldOffset(16)]
            public uint SizeOfRawData;
            [FieldOffset(20)]
            public uint PointerToRawData;
            [FieldOffset(24)]
            public uint PointerToRelocations;
            [FieldOffset(28)]
            public uint PointerToLinenumbers;
            [FieldOffset(32)]
            public ushort NumberOfRelocations;
            [FieldOffset(34)]
            public ushort NumberOfLinenumbers;
            [FieldOffset(36)]
            public DataSectionFlags Characteristics;

            public string Section => new string(Name)?.Trim()?.Replace("\u0000", "");
        }

        [Flags]
        public enum DataSectionFlags : uint
        {
            /// <summary>
            /// Reserved for future use.
            /// </summary>
            TypeReg = 0x00000000,
            /// <summary>
            /// Reserved for future use.
            /// </summary>
            TypeDsect = 0x00000001,
            /// <summary>
            /// Reserved for future use.
            /// </summary>
            TypeNoLoad = 0x00000002,
            /// <summary>
            /// Reserved for future use.
            /// </summary>
            TypeGroup = 0x00000004,
            /// <summary>
            /// The section should not be padded to the next boundary. This flag is obsolete and is replaced by IMAGE_SCN_ALIGN_1BYTES. This is valid only for object files.
            /// </summary>
            TypeNoPadded = 0x00000008,
            /// <summary>
            /// Reserved for future use.
            /// </summary>
            TypeCopy = 0x00000010,
            /// <summary>
            /// The section contains executable code.
            /// </summary>
            ContentCode = 0x00000020,
            /// <summary>
            /// The section contains initialized data.
            /// </summary>
            ContentInitializedData = 0x00000040,
            /// <summary>
            /// The section contains uninitialized data.
            /// </summary>
            ContentUninitializedData = 0x00000080,
            /// <summary>
            /// Reserved for future use.
            /// </summary>
            LinkOther = 0x00000100,
            /// <summary>
            /// The section contains comments or other information. The .drectve section has this type. This is valid for object files only.
            /// </summary>
            LinkInfo = 0x00000200,
            /// <summary>
            /// Reserved for future use.
            /// </summary>
            TypeOver = 0x00000400,
            /// <summary>
            /// The section will not become part of the image. This is valid only for object files.
            /// </summary>
            LinkRemove = 0x00000800,
            /// <summary>
            /// The section contains COMDAT data. For more information, see section 5.5.6, COMDAT Sections (Object Only). This is valid only for object files.
            /// </summary>
            LinkComDat = 0x00001000,
            /// <summary>
            /// Reset speculative exceptions handling bits in the TLB entries for this section.
            /// </summary>
            NoDeferSpecExceptions = 0x00004000,
            /// <summary>
            /// The section contains data referenced through the global pointer (GP).
            /// </summary>
            RelativeGp = 0x00008000,
            /// <summary>
            /// Reserved for future use.
            /// </summary>
            MemPurgeable = 0x00020000,
            /// <summary>
            /// Reserved for future use.
            /// </summary>
            Memory16Bit = 0x00020000,
            /// <summary>
            /// Reserved for future use.
            /// </summary>
            MemoryLocked = 0x00040000,
            /// <summary>
            /// Reserved for future use.
            /// </summary>
            MemoryPreload = 0x00080000,
            /// <summary>
            /// Align data on a 1-byte boundary. Valid only for object files.
            /// </summary>
            Align1Bytes = 0x00100000,
            /// <summary>
            /// Align data on a 2-byte boundary. Valid only for object files.
            /// </summary>
            Align2Bytes = 0x00200000,
            /// <summary>
            /// Align data on a 4-byte boundary. Valid only for object files.
            /// </summary>
            Align4Bytes = 0x00300000,
            /// <summary>
            /// Align data on an 8-byte boundary. Valid only for object files.
            /// </summary>
            Align8Bytes = 0x00400000,
            /// <summary>
            /// Align data on a 16-byte boundary. Valid only for object files.
            /// </summary>
            Align16Bytes = 0x00500000,
            /// <summary>
            /// Align data on a 32-byte boundary. Valid only for object files.
            /// </summary>
            Align32Bytes = 0x00600000,
            /// <summary>
            /// Align data on a 64-byte boundary. Valid only for object files.
            /// </summary>
            Align64Bytes = 0x00700000,
            /// <summary>
            /// Align data on a 128-byte boundary. Valid only for object files.
            /// </summary>
            Align128Bytes = 0x00800000,
            /// <summary>
            /// Align data on a 256-byte boundary. Valid only for object files.
            /// </summary>
            Align256Bytes = 0x00900000,
            /// <summary>
            /// Align data on a 512-byte boundary. Valid only for object files.
            /// </summary>
            Align512Bytes = 0x00A00000,
            /// <summary>
            /// Align data on a 1024-byte boundary. Valid only for object files.
            /// </summary>
            Align1024Bytes = 0x00B00000,
            /// <summary>
            /// Align data on a 2048-byte boundary. Valid only for object files.
            /// </summary>
            Align2048Bytes = 0x00C00000,
            /// <summary>
            /// Align data on a 4096-byte boundary. Valid only for object files.
            /// </summary>
            Align4096Bytes = 0x00D00000,
            /// <summary>
            /// Align data on an 8192-byte boundary. Valid only for object files.
            /// </summary>
            Align8192Bytes = 0x00E00000,
            /// <summary>
            /// The section contains extended relocations.
            /// </summary>
            LinkExtendedRelocationOverflow = 0x01000000,
            /// <summary>
            /// The section can be discarded as needed.
            /// </summary>
            MemoryDiscardable = 0x02000000,
            /// <summary>
            /// The section cannot be cached.
            /// </summary>
            MemoryNotCached = 0x04000000,
            /// <summary>
            /// The section is not pageable.
            /// </summary>
            MemoryNotPaged = 0x08000000,
            /// <summary>
            /// The section can be shared in memory.
            /// </summary>
            MemoryShared = 0x10000000,
            /// <summary>
            /// The section can be executed as code.
            /// </summary>
            MemoryExecute = 0x20000000,
            /// <summary>
            /// The section can be read.
            /// </summary>
            MemoryRead = 0x40000000,
            /// <summary>
            /// The section can be written to.
            /// </summary>
            MemoryWrite = 0x80000000
        }

        #endregion File Header Structures

        #region Private Fields

        /// <summary>
        /// The DOS header
        /// </summary>
        private readonly ImageDosHeader _dosHeader;
        /// <summary>
        /// The file header
        /// </summary>
        private ImageFileHeader _fileHeader;
        /// <summary>
        /// Optional 32 bit file header 
        /// </summary>
        private readonly ImageOptionalHeader32 _optionalHeader32;
        /// <summary>
        /// Optional 64 bit file header 
        /// </summary>
        private readonly ImageOptionalHeader64 _optionalHeader64;
        /// <summary>
        /// Image Section headers. Number of sections is in the file header.
        /// </summary>
        private readonly ImageSectionHeader[] _imageSectionHeaders;

        #endregion Private Fields

        #region Public Methods

        internal PeHeaderReader(string filePath)
        {
            // Read in the DLL or EXE and get the timestamp
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var reader = new BinaryReader(stream);
                _dosHeader = FromBinaryReader<ImageDosHeader>(reader);

                // Add 4 bytes to the offset
                stream.Seek(_dosHeader.ELfanew, SeekOrigin.Begin);

                var ntHeadersSignature = reader.ReadUInt32();
                _fileHeader = FromBinaryReader<ImageFileHeader>(reader);
                if (Is32BitHeader)
                {
                    _optionalHeader32 = FromBinaryReader<ImageOptionalHeader32>(reader);
                }
                else
                {
                    _optionalHeader64 = FromBinaryReader<ImageOptionalHeader64>(reader);
                }

                _imageSectionHeaders = new ImageSectionHeader[_fileHeader.NumberOfSections];
                for (var headerNo = 0; headerNo < _imageSectionHeaders.Length; ++headerNo)
                {
                    _imageSectionHeaders[headerNo] = FromBinaryReader<ImageSectionHeader>(reader);
                }
            }
        }

        /// <summary>
        /// Gets the header of the .NET assembly that called this function
        /// </summary>
        /// <returns></returns>
        internal static PeHeaderReader GetCallingAssemblyHeader()
        {
            // Get the path to the calling assembly, which is the path to the
            // DLL or EXE that we want the time of
            var filePath = System.Reflection.Assembly.GetCallingAssembly().Location;

            // Get and return the timestamp
            return new PeHeaderReader(filePath);
        }

        /// <summary>
        /// Gets the header of the .NET assembly that called this function
        /// </summary>
        /// <returns></returns>
        internal static PeHeaderReader GetAssemblyHeader()
        {
            // Get the path to the calling assembly, which is the path to the
            // DLL or EXE that we want the time of
            var filePath = System.Reflection.Assembly.GetAssembly(typeof(PeHeaderReader)).Location;

            // Get and return the timestamp
            return new PeHeaderReader(filePath);
        }

        /// <summary>
        /// Reads in a block from a file and converts it to the struct
        /// type specified by the template parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        internal static T FromBinaryReader<T>(BinaryReader reader)
        {
            // Read in a byte array
            var bytes = reader.ReadBytes(Marshal.SizeOf(typeof(T)));

            // Pin the managed memory while, copy it out the data, then unpin it
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            var theStructure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();

            return theStructure;
        }

        #endregion Public Methods

        #region Properties

        /// <summary>
        /// Gets if the file header is 32 bit or not
        /// </summary>
        public bool Is32BitHeader
        {
            get
            {
                const ushort imageFile32BitMachine = 0x0100;
                return (imageFile32BitMachine & FileHeader.Characteristics) == imageFile32BitMachine;
            }
        }

        /// <summary>
        /// Gets the file header
        /// </summary>
        public ImageFileHeader FileHeader => _fileHeader;

        /// <summary>
        /// Gets the optional header
        /// </summary>
        public ImageOptionalHeader32 OptionalHeader32 => _optionalHeader32;

        /// <summary>
        /// Gets the optional header
        /// </summary>
        public ImageOptionalHeader64 OptionalHeader64 => _optionalHeader64;

        public ImageSectionHeader[] ImageSectionHeaders => _imageSectionHeaders;

        #endregion Properties
    }
}
