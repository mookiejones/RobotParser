using System;
using System.IO;
using System.Text;

namespace RobotParser.Core
{
    [Serializable]
    public class CoreFileStream:CoreStringStream
    {
        protected string fileName;

	public override string SourceName
	{
		get
		{
			return this.fileName;
		}
	}

	public CoreFileStream(string fileName)
		: this(fileName, null)
	{
	}

	public CoreFileStream(string fileName, Encoding encoding)
	{
		this.fileName = fileName;
		this.Load(fileName, encoding);
	}

	public virtual void Load(string fileName, Encoding encoding)
	{
		if (fileName != null)
		{
			string text = (encoding != null) ? File.ReadAllText(fileName, encoding) : File.ReadAllText(fileName);
			base.data = text.ToCharArray();
			base.n = base.data.Length;
		}
	}
    }
}