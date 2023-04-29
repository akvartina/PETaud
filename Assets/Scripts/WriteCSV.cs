using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; // to write to files
using System;

public class WriteCSV : MonoBehaviour
{
    string filename = ""; // filename of the csv
    public int cnt = 0;

    // Start is called before the first frame update
    void Start()
    {
        // generate filename: datapath, the Data folder + a uid that we generate here
        filename = Application.dataPath + "/Data/" + Guid.NewGuid() + ".csv";
    }


	
    public void MakeCSV(List<string> data, string header)
    {
        Debug.Log("save");
        TextWriter csvFile = new StreamWriter(filename, false);
        csvFile.WriteLine(header);

				
        for (int i = 0; i < data.Count; i++)
        {
            Debug.Log("now");
            csvFile.WriteLine(data[i]);
                
        } 
			
        csvFile.Close();
    }
}