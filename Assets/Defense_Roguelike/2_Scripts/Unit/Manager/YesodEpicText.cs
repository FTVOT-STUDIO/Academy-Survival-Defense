using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class YesodEpicText
{
    public static readonly string[] Chapters = {
@"Present reality is not aligned with what was preordained
but
Do not paint me a believer of deities
Meaning exists without the divine
Meaning equates to truth
and
The only truth is that there is no truth",
@"The dictation of perception belongs to us
and us alone
This does not mean that meaning fluctuates
from one conscience to another
Meaning is constant
Meaning equates to truth
and
The only truth is that there is no truth",
@"A pursuit of freedom is in vain if one chases something
larger than themself
All things beyond conscience are fiction
True freedom does not exist in circumstance
It exists in thought
however
Neither is of more significance
For thought is circumstance
And circumstance to those unaltered
By existential disquiet is frightening",
@"The only truth is that there is no truth
And that truth is parallel to our meaning
Existing without the divine
This is my only belief
One of verity
One of freedom
One of a menace" };
    public static readonly string[] Fragments = Build();
    static string[] Build()
    {
        var result = new List<string>();
        for (int chapter = 0; chapter < 4; chapter++)
        {
            string text = Chapters[chapter];
            var words = Regex.Matches(text, @"\S+");
            int groups = chapter == 3 ? 24 : 25;
            int count = chapter == 3 ? words.Count - 4 : words.Count;
            for (int i = 0; i < groups; i++)
            {
                int first = i * count / groups;
                int next = (i + 1) * count / groups;
                int end = next < words.Count ? words[next].Index : text.Length;
                result.Add(text.Substring(words[first].Index, end - words[first].Index));
            }
            if (chapter == 3) result.Add("One of a menace");
        }
        return result.ToArray();
    }
}
