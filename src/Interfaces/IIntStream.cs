namespace RobotParser.Interfaces
{
    public interface IIntStream
{
	int Index
	{
		get;
	}

	int Count
	{
		get;
	}

	string SourceName
	{
		get;
	}

	void Consume();

	int LA(int i);

	int Mark();

	void Rewind(int marker);

	void Rewind();

	void Release(int marker);

	void Seek(int index);
}
}