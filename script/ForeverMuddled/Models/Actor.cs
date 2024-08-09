using BymlLibrary;
using BymlLibrary.Nodes.Containers;
using ForeverMuddled.Helpers;
using Revrs.Buffers;
using SarcLibrary;
using System.Diagnostics.CodeAnalysis;

namespace ForeverMuddled.Models;

class Actor
{
    private const string ACTOR_PARAM_NAME = "Actor/{0}.engine__actor__ActorParam.bgyml";

    private readonly string _filePath;
    private readonly string _actorName;
    private readonly Sarc _actorPack;
    private readonly int _dictionaryId;

    public Actor(string filePath)
    {
        _filePath = filePath;
        _actorName = Path.GetFileNameWithoutExtension(filePath.AsSpan()[..^3]).ToString();

        using ArraySegmentOwner<byte> actorPackData = DataHelper.ReadAndDecompress(filePath, out _dictionaryId);
        _actorPack = Sarc.FromBinary(actorPackData.Segment);
    }
}
