﻿using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

/*
    Copyright (c) 2017 Sloan Kelly

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

internal sealed class MapReader
{
    [HideInInspector]
    public Dictionary<ulong, OsmNode> nodes;

    [HideInInspector]
    public List<OsmWay> ways;
    
    [HideInInspector]
    public OsmBounds bounds;

    /// <summary>
    /// Load the OpenMap data resource file.
    /// </summary>
    /// <param name="resourceFile">Path to the resource file. The file must exist.</param>
	public void Read(string resourceFile)
    {
        nodes = new Dictionary<ulong, OsmNode>();
        ways = new List<OsmWay>();

        var xmlText = File.ReadAllText(resourceFile);

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xmlText);

        SetBounds(doc.SelectSingleNode("/osm/bounds"));
        GetNodes(doc.SelectNodes("/osm/node"));
        GetWays(doc.SelectNodes("/osm/way"));

        float minx = (float)MercatorProjection.lonToX(bounds.MinLon);
        float maxx = (float)MercatorProjection.lonToX(bounds.MaxLon);
        float miny = (float)MercatorProjection.latToY(bounds.MinLat);
        float maxy = (float)MercatorProjection.latToY(bounds.MaxLat);
    }

    void GetWays(XmlNodeList xmlNodeList)
    {
        foreach (XmlNode node in xmlNodeList)
        {
            OsmWay way = new OsmWay(node);
            ways.Add(way);
        }
    }

    void GetNodes(XmlNodeList xmlNodeList)
    {
        foreach (XmlNode n in xmlNodeList)
        {
            OsmNode node = new OsmNode(n);
            nodes[node.ID] = node;
        }
    }

    void SetBounds(XmlNode xmlNode)
    {
        bounds = new OsmBounds(xmlNode);
    }
}
