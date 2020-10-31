/**************************************************************************
* Copyright (C) echoAR, Inc. 2018-2020.                                   *
* echoAR, Inc. proprietary and confidential.                              *
*                                                                         *
* Use subject to the terms of the Terms of Service available at           *
* https://www.echoar.xyz/terms, or another agreement                      *
* between echoAR, Inc. and you, your company or other organization.       *
***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomBehaviour : MonoBehaviour
{
    [HideInInspector]
    public Entry entry;

    /// <summary>
    /// EXAMPLE BEHAVIOUR
    /// Queries the database and names the object based on the result.
    /// </summary>

    private string[] shopsNames = { 
                                    "Zara",
                                    "Pantaloons",
                                    "Max",
                                    "Reliance Trends",
                                    "Mango",
                                    "Wrogn" };
    private Dictionary<string, Data> dataMap = new Dictionary<string, Data>();
    private int scale = 1;

    public class Data
    {
        public int cases;
        public GameObject graph;
        
        public Data(int cases, GameObject graph)
        {
            this.cases = cases;
            this.graph = graph;
        }
    }


    // Use this for initialization
    void Start()
    {
        // Add RemoteTransformations script to object and set its entry
        this.gameObject.AddComponent<RemoteTransformations>().entry = entry;

        // Qurey additional data to get the name
        string value = "";
        if (entry.getAdditionalData() != null && entry.getAdditionalData().TryGetValue("name", out value))
        {
            // Set name
            this.gameObject.name = value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (entry.getAdditionalData() != null) {
            // Qurey additional data to get the scale
            string scaleString = "";
            if (entry.getAdditionalData().TryGetValue("scale", out scaleString))
            {
                scale = int.Parse(scaleString);
            }
            // Iterate over all shops
            foreach (string shop in shopsNames)
            {
                // Qurey additional data to get shops data
                string casesString = "";
                if (entry.getAdditionalData().TryGetValue(shop, out casesString))
                {
                    // Parse number of cases shop
                    int cases = int.Parse(casesString);
                    // Check for existing data about the 
                    Data data;
                    if (dataMap.TryGetValue(shop, out data)){
                        if (data.cases != cases){
                            // Scale graph
                            data.graph.transform.localScale = new Vector3(1f, cases, 1f);
                        }
                        data.graph.transform.position = this.gameObject.transform.position + new Vector3(1.5f * scale + 2.5f * System.Array.IndexOf(shopsNames, shop), -scale, 0);
                    } else {
                        // Create bar
                        GameObject bar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                        bar.GetComponent<Renderer>().material.color = new Color(1,0,0,0.1f);
                        // Create base
                        GameObject barBase = new GameObject("BarBase " + shop);
                        barBase.AddComponent<MeshFilter>();
                        // Set base position
                        barBase.transform.position = new Vector3(0, -bar.transform.localScale.y, 0);
                        // Set base as parent
                        bar.transform.parent = barBase.transform;
                        // Set data graph
                        data = new Data(cases, barBase);
                        // Scale graph
                        data.graph.transform.localScale = new Vector3(1f, cases, 1f);
                        // Set graph name
                        data.graph.name = shop;
                        // Set graph location
                        data.graph.transform.position = this.gameObject.transform.position + new Vector3(1.5f * scale + 2.5f * System.Array.IndexOf(shopsNames, shop), -scale, 0);
                        // Add text
                        GameObject text = new GameObject();
                        TextMesh t = text.AddComponent<TextMesh>();
                        t.text = shop;
                        t.fontSize = 100;
                        text.name = "Text " + shop;
                        text.transform.localScale = 0.1f * Vector3.one;
                        text.transform.position = data.graph.transform.position;
                        text.transform.eulerAngles = new Vector3(0, 0, 90);
                        // Add to map
                        dataMap.Add(shop, data);
                    }
                } else {
                    dataMap.Remove(shop);
                    Destroy(GameObject.Find(shop));
                }
            }
        }
    }
}