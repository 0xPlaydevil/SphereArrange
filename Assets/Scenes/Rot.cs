using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rot : MonoBehaviour
{
	public string picPath="";
	public float rotSpeed=20;
	public float radius=100;
	public int rows=3;

	// float width;
	// float height;
	float ratio;

	public GameObject imgTemp;
	SpriteRenderer spriteR;
	List<Transform> images= new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
    	spriteR= imgTemp.GetComponent<SpriteRenderer>();
    	ratio= spriteR.size.x/spriteR.size.y;
    	var sprites= Resources.LoadAll<Sprite>(picPath);
        for(int i=0;i<sprites.Length;++i)
        {
        	images.Add(GameObject.Instantiate(imgTemp).transform);
        	images[i].parent= imgTemp.transform.parent;
        	images[i].GetComponent<SpriteRenderer>().sprite=sprites[i];
        	images[i].gameObject.SetActive(true);
        }
        // Arrange(30*Mathf.Deg2Rad);
        print($"image count: {images.Count}");
        float w= CalcWidth(rows);
        print($"final width: {w}");
        Arrange(rows,w);
        print($"final image count: {CalcCapacity(rows,w)}");
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up*rotSpeed*Time.deltaTime);
    }

    float CalcWidth(int rows)
    {
    	int cols= images.Count/rows+1;
    	float refWidth= Mathf.PI*2*radius/cols;
    	float min= refWidth/2;
    	float max= refWidth*2;
    	while(CalcCapacity(rows,min)<images.Count)
    	{
    		min= min/2;
    	}
    	while(CalcCapacity(rows,max)>images.Count)
    	{
    		max=max*2;
    	}
    	float curWidth=0;
    	int times=0;
    	const int maxTimes=30;
    	int curN=0;
    	do{
    		curWidth= Mathf.Lerp(min,max,0.5f);
			curN=CalcCapacity(rows,curWidth);
			if(curN<images.Count)
			{
				max=curWidth;
			}
			else
			{
				min=curWidth;
			}
			++times;
    	}while(curN!=images.Count && times<maxTimes);
    	print($"bisearch loop times: {times} of max: {maxTimes}");
    	return curWidth;
    }
    
    void Arrange(int rows,float width)
    {
    	int loops= (rows+1)/2;
    	float latiU= ChordAngle(radius, width/ratio);
    	int idx=0;
    	for(int i=0;i<loops;++i)
    	{
    		float lati= (rows%2==0? latiU: latiU/2)+ latiU*i;
    		int n= Mathf.FloorToInt(Mathf.PI*2/ChordAngle(LatiRadius(radius,lati),width));

    		lati=(rows%2==0? latiU/2: 0)+ latiU*i;
    		float longiU= Mathf.PI*2/n;
    		for(int j=0;j<n;++j)
    		{
    			images[idx].localPosition= PointBySphere(lati,longiU*j,radius,spriteR.size.x,spriteR.size.y);
    			++idx;
    		}
    		if(i!=0 || rows%2==0)
    		{
	    		for(int j=0;j<n && idx<images.Count;++j)
	    		{
	    			images[idx].localPosition= PointBySphere(-lati,longiU*j,radius,spriteR.size.x,spriteR.size.y);
	    			++idx;
	    		}
    		}
    	}
    	for(int k=0;k<images.Count;++k)
    	{
    		images[k].localRotation= Quaternion.LookRotation(images[k].localPosition);
    		images[k].GetComponent<SpriteRenderer>().size=new Vector2(width,width/ratio);
    	}
    }

    Vector3 PointBySphere(float lati, float longi, float radius, float width, float height)
    {
    	Vector3 pos;
    	pos.y= radius*Mathf.Sin(lati);
    	float r= LatiRadius(radius,lati);
    	pos.x= r*Mathf.Sin(longi);
    	pos.z= r*Mathf.Cos(longi);
    	return pos;
    }

    int CalcCapacity(int rows, float width)
    {
    	int loops= (rows+1)/2;
    	int capac=0;
    	float latiU= ChordAngle(radius, width/ratio);
    	for(int i=0;i<loops;++i)
    	{
    		float lati= (rows%2==0? latiU: latiU/2)+ latiU*i;
    		int n= Mathf.FloorToInt(Mathf.PI*2/ChordAngle(LatiRadius(radius,lati),width));
    		if(i!=0 || rows%2==0)
    		{
    			n=n*2;
    		}
    		capac+=n;
    	}
    	return capac;
    }

    float ChordAngle(float radius, float chord)
    {
    	return Mathf.Asin(chord/2/radius)*2;
    }

    float LatiRadius(float radius, float lati)
    {
    	return radius*Mathf.Cos(lati);
    }

    void Arrange(float angleLimit)
    {
    	float limSin= Mathf.Abs(Mathf.Sin(angleLimit));
    	for(int i=0;i<images.Count;)
    	{
	    	Vector3 pos= Random.onUnitSphere;
	    	float aglSin= Mathf.Abs(pos.y);
	    	if(aglSin<limSin)
	    	{
	    		images[i].localPosition=pos*radius;
	    		images[i].localRotation= Quaternion.LookRotation(images[i].localPosition);
	    		++i;
	    	}
    	}
    }
}
