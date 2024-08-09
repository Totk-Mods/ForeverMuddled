using BymlLibrary;
using BymlLibrary.Nodes.Containers;
using ForeverMuddled.Helpers;
using Revrs;
using Revrs.Buffers;
using SarcLibrary;
using System.Diagnostics.CodeAnalysis;
using TotkCommon;

namespace ForeverMuddled.Models;

public enum MuddleMode
{
    Set,
    Add,
    Multiply
}

class Actor
{
    private const string ACTOR_PARAM_NAME = "Actor/{0}.engine__actor__ActorParam.bgyml";
    private const float CONFUSION_DELAY_DEFAULT = 900.0f;

    private readonly string _actorName;
    private readonly Sarc _actorPack;
    private readonly int _dictionaryId;

    public Actor(string filePath)
    {
        _actorName = Path.GetFileNameWithoutExtension(filePath.AsSpan()[..^3]).ToString();

        using ArraySegmentOwner<byte> actorPackData = DataHelper.ReadAndDecompress(filePath, out _dictionaryId);
        _actorPack = Sarc.FromBinary(actorPackData.Segment);
    }

    public void Save(string outputActorFolder)
    {
        Directory.CreateDirectory(outputActorFolder);
        string output = Path.Combine(outputActorFolder, $"{_actorName}.pack.zs");

        using MemoryStream ms = new();
        _actorPack.Write(ms, Endianness.Little);

        byte[] raw = ms.ToArray();
        FileStream fs = File.Create(output);
        using ArraySegmentOwner<byte> compressed = ArraySegmentOwner<byte>.Allocate(raw.Length);
        int size = Totk.Zstd.Compress(raw, compressed.Segment, _dictionaryId);
        fs.Write(compressed.Segment[..size]);
    }

    public bool Muddle(float muddleFactor, MuddleMode muddleMode = MuddleMode.Set)
    {
        if (!HasConditionParam(out string? conditionRef)) {
            return false;
        }

    Retry:
        if (!_actorPack.TryGetValue(conditionRef, out ArraySegment<byte> conditionParamData)) {
            Console.WriteLine(value: $"""
                The referenced ConditionParam '{conditionRef}' could not be found in the actor '{_actorName}'
                """);
            return false;
        }

        Byml conditionParam = Byml.FromBinary(conditionParamData, out Endianness endianness, out ushort version);
        BymlMap param = conditionParam.GetMap();

        if (!param.TryGetValue("IsConfusion", out Byml? isConfusion) || isConfusion.GetBool() is false) {
            if (TryGetParentRef(param) is not string parentRef) {
                return false;
            }

            conditionRef = parentRef;
            goto Retry;
        }

        if (!param.TryGetValue("ConfusionDelay", out Byml? oldMuddleValue)) {
            oldMuddleValue = CONFUSION_DELAY_DEFAULT;
        }

        param["ConfusionDelay"] = muddleMode switch {
            MuddleMode.Set => muddleFactor,
            MuddleMode.Add => (float)(oldMuddleValue.GetFloat() + muddleFactor),
            MuddleMode.Multiply => (float)(oldMuddleValue.GetFloat() * muddleFactor),
            _ => throw new NotSupportedException($"""
                Unsupported MuddleMode '{muddleMode}'
                """)
        };

        _actorPack[conditionRef] = conditionParam.ToBinary(endianness, version);
        return true;
    }

    private bool HasConditionParam([MaybeNullWhen(false)] out string conditionRef)
    {
        string actorParamFileName = string.Format(ACTOR_PARAM_NAME, _actorName);
        if (!_actorPack.TryGetValue(actorParamFileName, out ArraySegment<byte> actorParamData)) {
            conditionRef = null;
            return false;
        }
        
        while (true) {
            Byml actorParam = Byml.FromBinary(actorParamData);
            BymlMap actorParamRoot = actorParam.GetMap();

            if (actorParamRoot["Components"].GetMap().TryGetValue("ConditionRef", out Byml? conditionRefNode) && conditionRefNode.Value is string conditionRefStr) {
                if (string.IsNullOrEmpty(conditionRefStr) || conditionRefStr[0] is not '?') {
                    break;
                }

                conditionRef = conditionRefStr[1..];
                return true;
            }

            if (TryGetParentRef(actorParamRoot) is not string parentRef) {
                break;
            }

            actorParamData = _actorPack[parentRef];
        }

        conditionRef = default;
        return false;
    }

    private string? TryGetParentRef(BymlMap map)
    {
        if (!map.TryGetValue("$parent", out Byml? parentRefNode)) {
            return null;
        }

        string parentRef = parentRefNode.GetString()[5..];

        // Skip Resident Actor
        if (parentRef is "Actor/FarActorTemplate.engine__actor__ActorParam.gyml") {
            return null;
        }

        return Path.ChangeExtension(parentRef, "bgyml");
    }
}
