using JetBrains.Annotations;
using Syncfusion.Emf.Exceptions;
using Syncfusion.Emf.Records;
using Syncfusion.Emf.Records.Control.Eof;
using Syncfusion.Emf.Records.Control.Header;

namespace Syncfusion.Emf;

 
public sealed class Metafile
{
    /// <summary>
    /// Header of the EMF file
    /// </summary>
    public EmfMetafileHeader Header { get; }

    /// <summary>
    /// Records of the EMF file
    /// </summary>
    public IReadOnlyList<MetafileRecord> Records { get; }

    /// <summary>
    /// End of file record
    /// </summary>
    public EmrEof Eof { get; }

    private Metafile(EmfMetafileHeader header, IReadOnlyList<MetafileRecord> records, EmrEof eof)
    {
        Header = header;
        Records = records;
        Eof = eof;
    }

    public static Metafile LoadFromFile(string path)
    {
        using var fs = File.OpenRead(path);
        var header = (EmfMetafileHeader)MetafileRecord.Parse(fs);
        fs.Seek(header.Size - fs.Position, SeekOrigin.Current);

        var records = new List<MetafileRecord>((int)header.Records - 2);

        while (fs.Position < fs.Length)
        {
            var record = MetafileRecord.Parse(fs);
            if (record is EmrEof eof)
            {
                return new Metafile(header, records, eof);
            }

            records.Add(record);
        }

        throw new EmfParseException("EOF record not found");
    }

}