//Coded by lschneck, meant to implement a linked list of event nodes
//could break if linked list is circular
//or if move next doesn't return null when i expect
using System;
public class List{
    	private LinkedList<Node> eventList;
	public List(){
		eventList = new LinkedList<Node>(); 
	}
	public int addEvent(DateTime startTime,DateTime endTime,string title,int type,string extra,string detail){
		//create event based on given data
		if(detail.Equals(null) && type == null){
			Node temp = new Node(title, startTime.ToBinary(), endTime.ToBinary());
		}else if(type == null){
			Node temp = new Node(title, detail, startTime.ToBinary(), endTime.ToBinary());
		}else if(detail.Equals(null)){
			Node temp = new Node(title, startTime.ToBinary(), endTime.ToBinary(), type, extra);
		}else{
			Node temp = new Node(title, detail,startTime.ToBinary(), endTime.ToBinary(), type, extra);
		}
		//add the event into list sorted by startTime
		if(eventList.Count!=0){
			int j=0;
			LinkedList<Node>.Enumerator e = eventList.GetEnumerator();
			while(e.MoveNext() && j!=1){
				if(DateTime(e.getStart()) <= DateTime(e.getStart())){
					//check the end time
					if(!e.checkRepeat(temp.getStart(), temp.getEnd())){
						return -1;
					}
					//add to non-first and non-last point in list
					if(DateTime(e.getStart()) != DateTime(eventList.First.getStart())){
			 			temp.addBefore(e);
			 		}else{
			 			//add to first point in list
			 			eventList.addFirst(temp);
			 		}
			 		j=1;
				}
			}
			if(j!=1){
			//add to end of list and check the end time
				if(!e.checkRepeat(temp.getStart(), temp.getEnd())){
					return -1;
				}
	 			eventList.AddLast(temp);
			}
		}else if(eventList.Count==0){
			eventList.addFirst(temp);
		}
		return 1;
	}
	public int removeEvent(DateTime startTime){
		LinkedList<Node>.Enumerator e = eventList.GetEnumerator();
		while(e.MoveNext()){
			if(DateTime(e.getStart()) == startTime){
				eventList.Remove(e);
				return 1;
			}
		}
		return -1;
	}
	public string returnEvent(DateTime startTime){
        	LinkedList<Node>.Enumerator e = eventList.GetEnumerator();
       		while(e.MoveNext()){
            		if(DateTime(e.getStart()) == startTime){
             	   	return e.getInfo();
      	      		}
   	    	 }
   	     	return "FAILED";
	}
	public string returnAll(){
        	string temp=""; 
        	LinkedList<Node>.Enumerator e = eventList.GetEnumerator();
        	while(e.MoveNext()){
            		temp = string.Concat(temp, e.getInfo());
        	}
        	return temp;
	}
}