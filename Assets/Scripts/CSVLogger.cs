using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Unity.Mathematics;
using UnityEngine;

// provide following feature
// create a file given a name also provide the 
// 
// serialize the data into csv format
public interface ICsvable
{
    public string Serialize ();
    public string Header(); // csv header for this class, return each field name
}
[Serializable]
public class UserInfo : ICsvable
{
    [SerializeField]
    private string userID;
    [SerializeField]
    private string gender;
    [SerializeField]
    private string age;
    [SerializeField]
    private string ethinicity;
    [SerializeField]
    private string handness;
    public string Serialize()
    {
        // return $"{userID},{gender},{age},{ethinicity},{handness}";
        var fields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        return string.Join(",", fields.Select(f => f.GetValue(this)));
    }

    public string Header()
    {
        // return $"userID,gender,age,ethinicity,handness";
        var fields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        return string.Join(",", fields.Select(f => f.Name));
    }
}

public class EntryInfo : ICsvable
{
    public string respkeys;
    public int condition;
    public string correctness;
    public string reactionTime;
    public List<int> stimuli_locs;
    public int TotolSetSize{
        get{
            return stimuli_locs.Count;
        }
    }
    public string trialType;
    public string Serialize()
    {

        return $"{respkeys},{condition},{correctness},{reactionTime},\"[{string.Join(",", stimuli_locs)}]\",{TotolSetSize},{trialType}";
    }

    // csv header for this class, return each field name
    public string Header()
    {
        return "respkeys,condition,correctness,reactionTime,stimuli_locs,TotolSetSize,trialType";
    }
}
public class CSVLogger : MonoBehaviour
{
    public string rootPath;
    public List<string> header;
    private string path;
    [SerializeField]
    private UserInfo userInfo;
    
    // called when the game starts
    public void Initialize(string fileName)
    {
        // unity path
        path = Path.GetFullPath(Path.Combine(Application.dataPath,rootPath, $"{fileName}.csv"));//rootPath + "/" + fileName + ".csv";
        Debug.Log(path);
        // create a file
        using StreamWriter sw = new StreamWriter(path);
        sw.WriteLine(string.Join(",", header));

        // write the header
    }

    public void AddData(string newEntry)
    {
        using StreamWriter sw = new StreamWriter(path, true);
        sw.WriteLine(newEntry);
    }

    public void AddDataWithUserInfo(string newEntry)
    {
        using StreamWriter sw = new StreamWriter(path, true);
        sw.WriteLine($"{newEntry},{userInfo.Serialize()}");
    }

    [ContextMenu("Header Generator(it will overwrite the header)")]
    public void HeaderGenerator()
    {
        List<string> newHeader = new List<string>();
        // first use the header from the entryInfo then add the user info
        var entryInfo = new EntryInfo();
        newHeader.AddRange(entryInfo.Header().Split(','));
        newHeader.AddRange(userInfo.Header().Split(','));
        header = newHeader;
    }
}


public class Solution {
    public int NumSpecial(int[][] mat)
    {
        int m = mat.Length;
        int n = mat[0].Length;
        // each row how many 1;
        int[] row = new int[m];
        Array.Fill(row,-1);
        // each col how many 1;
        int[] col = new int[n];
        Array.Fill(col,-1);
        for (int i = 0; i < m; i++)
        {
            int count = 0;
            for (int j = 0; j < n; j++)
            {
                if (mat[i][j] == 1)
                {
                    count++;
                    if (count >= 2)
                    {
                        break;
                    }
                }
            }

            row[i] = count;
        }

        for (int j = 0; j < n; j++)
        {
            int count = 0;
            for (int i = 0; i < m; i++)
            {
                if (mat[i][j] == 1)
                {
                    count++;
                    if (count >= 2)
                    {
                        break;
                    }
                }
            }

            col[j] = count;
        }

        int res = 0;
        for (int i = 0; i < m; i++)
        {
            if (row[i] != 1)
            {
                continue;
            }
            for (int j = 0; j < n; j++)
            {
                if (mat[i][j] == 1)
                {
                    if (col[j] == 1)
                    {
                        res++;
                    }
                    break;
                    
                }
            }
        }

        return res;
    }
}