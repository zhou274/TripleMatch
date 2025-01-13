// this thing is inevitable if you want to use a record type in Unity
using System.ComponentModel;
namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class IsExternalInit { }
}
public record MatchedThree(MatchingFlyingIcon left, MatchingFlyingIcon mid, MatchingFlyingIcon right);
