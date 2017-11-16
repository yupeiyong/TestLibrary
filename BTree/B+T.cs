using System; 
using System.Xml; 
using System.Xml.Serialization; 
using System.IO;
using System.Text;
using System.Collections;

namespace BPlus_Tree 
{
    #region Class Pair
    public class Pair           
     { 
        [XmlElement("pointer")] 
        public object pointer;  
                                
        [XmlAttribute("key")] 
        public int key;         
                                
        public Pair(int key,object pointer) 
        { 
            this.key=key; 
            this.pointer=pointer; 
        } 
        public Pair() 
        {
        } 
     }  
    #endregion

    #region Class BPNode
    [XmlRoot("BPTroot")] 
    public class BPNode                    
    { 
        [XmlAttribute("isLeaf")] 
        public bool isLeaf;                
        [XmlElement("Pair")] 
        public Pair[] recarray;           
        [XmlAttribute("numrec")] 
        public int numrec;                  
        private BPNode leftptr,rightptr;    

        public void setPrev(BPNode LNode)  
        { 
            leftptr=LNode; 
        } 

        public BPNode getPrev() 
        { 
            return leftptr; 
        } 

        public void setNext(BPNode RNode)   
        { 
            rightptr=RNode; 
        } 

        public BPNode getNext() 
        { 
            return rightptr; 
        } 

        public bool isFull                  
        { 
            get 
            { 
                if(numrec==maxSize) 
                    return true; 
                else 
                    return false; 
            } 
        } 

        public int maxSize               
        { 
            get 
            { 
                if(isLeaf) 
                    return 5; 
                else 
                    return 4; 
            } 
        } 

        public BPNode()                  
        { 
            recarray=new Pair[5];
            for (int i = 0; i < 5; i++)
            {
                recarray[i] = new Pair();
            }
        } 
    }   
    #endregion 
    
    #region Class BPT
    public class BPT
    {
        #region Initialization Section
        public BPNode BPTroot;
        private int MinKey = int.MaxValue;

        public BPT()            
        { 
            BPTroot=new BPNode(); 
            BPTroot.recarray[0]=new Pair(0,"VIRTUAL RECORD");   
            BPTroot.numrec=1; 
            BPTroot.isLeaf=true; 
        } 
        #endregion
        
        #region Insert Function Group
        public void InsertBPT(int Key, object Pointer)
         {
             try
             {
                if (MinKey > Key)
                {
                    MinKey = Key;
                }

                Pair tmpPair = new Pair();
                tmpPair.key = Key;
                if (tmpPair.key <= 0) throw new Exception("Keys must be above zero!");
                tmpPair.pointer = Pointer;
                Pair retPair = inserthelp(BPTroot, tmpPair);
                if (retPair != null)
                {  
                    BPNode NewRoot = new BPNode();
                    NewRoot.numrec = 2;
                    NewRoot.isLeaf = false;
                    NewRoot.recarray[0].key = BPTroot.recarray[0].key;
                    NewRoot.recarray[0].pointer = BPTroot;
                    NewRoot.recarray[1].key = retPair.key;
                    NewRoot.recarray[1].pointer = retPair.pointer;
                    BPTroot = NewRoot;
                }
             }
             catch (Exception e)
             {
                 Console.WriteLine(e.Message);
             }
         }

        public void InsertBPT(int Key, ref Pair Pointer)
        {
            try
            {
                if (MinKey > Key)
                {
                    MinKey = Key;
                }

                Pair tmpPair = new Pair();
                tmpPair.key = Key;
                if (tmpPair.key <= 0) throw new Exception("Keys must be above zero!");
                tmpPair.pointer = Pointer;
                Pair retPair = inserthelp(BPTroot, tmpPair);
                if (retPair != null)
                {
                    BPNode NewRoot = new BPNode();
                    NewRoot.numrec = 2;
                    NewRoot.isLeaf = false;
                    NewRoot.recarray[0].key = BPTroot.recarray[0].key;
                    NewRoot.recarray[0].pointer = BPTroot;
                    NewRoot.recarray[1].key = retPair.key;
                    NewRoot.recarray[1].pointer = retPair.pointer;
                    BPTroot = NewRoot;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        } 
      
        public Pair inserthelp(BPNode root, Pair tmpPair) 
        { 
            Pair insPair; 
            int currec=binaryle(root.recarray,root.numrec,tmpPair.key); 
            if(root.isLeaf)                         
            { 
                if(root.recarray[currec].key==tmpPair.key) 
                {
                    Console.WriteLine("Duplicates are not allowed for primary key! " + tmpPair.key); 
                    return null; 
                } 
                else insPair=tmpPair; 
            } 
            else                                   
            { 
                insPair=inserthelp((BPNode)root.recarray[currec].pointer,tmpPair); 
                if(insPair==null)return null;      
            } 
           
            if(!root.isFull) 
            { 
                putinarray(root,currec,insPair); 
                return null; 
            } 
            else 
            { 
                BPNode tp=splitnode(root,currec,insPair); 
                return new Pair(tp.recarray[0].key,tp); 
            } 
        } 

       
        public void putinarray(BPNode root, int pos, Pair pair) 
        { 
            for(int i=root.numrec-1;i>pos;i--) 
            { 
                root.recarray[i+1].key=root.recarray[i].key; 
                root.recarray[i+1].pointer=root.recarray[i].pointer; 
            } 
            root.numrec++; 
            root.recarray[pos+1].key=pair.key; 
            root.recarray[pos+1].pointer=pair.pointer; 
        } 


        BPNode splitnode(BPNode root,int pos,Pair pair) 
        { 
            BPNode RNode=new BPNode(); 
            RNode.setNext(root.getNext()); 
            root.setNext(RNode); 
            RNode.setPrev(root);

            if (RNode.getNext() != null)
            {
                RNode.getNext().setPrev(RNode);
            }

            if (root.isLeaf)
            {
                RNode.numrec = root.numrec = 3;     //(5+1)/2 
                RNode.isLeaf = true;
            }
            else
            {
                RNode.numrec = 2; root.numrec = 3;
                RNode.isLeaf = false;
            }  
            try
            {
                if (pos < 2)
                {
                    int l;
                    if (root.isLeaf)
                    {
                        l = 4;
                    }
                    else
                    {
                        l = 3;
                    }
                    for (int i = l; i >= 2; i--)
                    {
                        RNode.recarray[i - 2].key = root.recarray[i].key;
                        RNode.recarray[i - 2].pointer = root.recarray[i].pointer;
                        
                    }
                    if (pos == 0)
                    {
                        root.recarray[2].key = root.recarray[1].key;
                        root.recarray[2].pointer = root.recarray[1].pointer;
                    }
                    root.recarray[pos + 1].key = pair.key;
                    root.recarray[pos + 1].pointer = pair.pointer;
                }
                else
                {
                    int l;
                    if (root.isLeaf)
                    {
                        l = 5;
                    }
                    else
                    {
                        l = 4;
                    }
                    for (int i = 3; i < l; i++)
                    {
                        if (i > pos)
                        {
                            RNode.recarray[i - 2].key = root.recarray[i].key;
                            RNode.recarray[i - 2].pointer = root.recarray[i].pointer;
                        }
                        else
                        {
                            RNode.recarray[i - 3].key = root.recarray[i].key;
                            RNode.recarray[i - 3].pointer = root.recarray[i].pointer;
                        }
                    }
                    RNode.recarray[pos - 2].key = pair.key;
                    RNode.recarray[pos - 2].pointer = pair.pointer;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return RNode; 
        } 
        #endregion 

        #region Find/View Function Group
        public object FindBPT(int MyKey)
        {
            try
            {
                int K = MyKey;
                int key = K;
                BPNode ResultNode = findhelp(BPTroot, ref K);         
                                                                       
                if (ResultNode.recarray[K].key == key)
                {
                    return ResultNode.recarray[K].pointer;
                }
                else
                { 
                    return null; 
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        } 
     
        
        public  BPNode findhelp(BPNode root,ref int K) 
        { 
            int currec=binaryle(root.recarray,root.numrec,K);
            if (root.isLeaf)        
            {
                K = currec;
                return root;
            }
            else
            {
                return findhelp((BPNode)root.recarray[currec].pointer, ref K);     
            }
        } 

 
        int binaryle(Pair[] recarray,int numrec,int K) 
        { 
            int i=1;          
            while(i<numrec)    
            {
                if (recarray[i].key > K)
                {
                    break;
                }
                else
                {
                    i++;
                }
            } 
            return i-1; 
        } 

        void ViewRange() 
        { 
            try 
            { 
                Console.Write("Range Start Key:"); 
                int K1=Convert.ToInt32(Console.ReadLine()); 
                int key=K1; 
                Console.Write("Range End Key:"); 
                int K2=Convert.ToInt32(Console.ReadLine()); 
                BPNode ResultNode=findhelp(BPTroot,ref key);        
                                                                   
                if (ResultNode.recarray[key].key == K1)
                {
                    Console.WriteLine("{0}\t{1}", K1, (string)ResultNode.recarray[key].pointer);
                }
                key++; 
                for(;;) 
                { 
                    if(key==ResultNode.numrec) 
                    { 
                        ResultNode=ResultNode.getNext(); 
                        key=0; 
                    }
                    if ((ResultNode != null) && (ResultNode.recarray[key].pointer != null) && (ResultNode.recarray[key].key <= K2))
                    {
                        Console.WriteLine("{0}\t{1}", ResultNode.recarray[key].key, (string)ResultNode.recarray[key].pointer);
                    }
                    else
                    {
                        break;
                    }
                    key++; 
                } 
            } 
            catch(Exception e) 
            { 
                Console.WriteLine(e.Message); 
            } 
        } 
        #endregion 

        #region Save/Load Function Group 
        void SaveBPT() 
        { 
            try 
            { 
                XmlSerializer serializerw = new XmlSerializer(typeof(BPNode)); 
                TextWriter writer = new StreamWriter("BPTExpr.xml"); 
                serializerw.Serialize(writer,BPTroot); 
                writer.Close(); 
            } 
            catch(Exception e) 
            { 
                Console.WriteLine(e.Message); 
            } 
        } 

        void LoadBPT() 
        { 
            try 
            { 
                XmlSerializer serializerr = new XmlSerializer(typeof(BPNode)); 
                FileStream fs = new FileStream("BPTExpr.xml", FileMode.Open); 
                BPTroot = (BPNode)serializerr.Deserialize(fs); 
                fs.Close(); 
                ReCreateLink(BPTroot); 
            } 
            catch(Exception e) 
            { 
                Console.WriteLine(e.Message); 
            } 
        } 

        void ReCreateLink(BPNode root) 
        { 
            if(root.isLeaf)     return;     
            if(!((BPNode)root.recarray[0].pointer).isLeaf)  //test bug
            for (int i = 0; i < root.numrec; i++)
            {
                ReCreateLink((BPNode)root.recarray[i].pointer);
            }

            for(int i=0;i<root.numrec-1;i++) 
            { 
                ((BPNode)root.recarray[i].pointer).setNext((BPNode)root.recarray[i+1].pointer); 
                ((BPNode)root.recarray[i+1].pointer).setPrev((BPNode)root.recarray[i].pointer); 
            } 
        } 
        #endregion 

        #region Delete Function Group 
        public void RemoveBPT(int Key)
        {
            try
            {
                int K = Key;
                if (K == 0) throw new Exception("Virtual Record is not allowed to delete!");

                int retvalue = removehelp(BPTroot, K, 0);
                if (retvalue == -2)
                    throw new Exception("Record Not Found");
                else if (retvalue == 0)
                    BPTroot = (BPNode)BPTroot.recarray[0].pointer;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        } 

        int removehelp(BPNode root,int K,int thispos) 
        { 
            int currec=binaryle(root.recarray,root.numrec,K); 

            if(root.isLeaf) 
            { 
            if(root.recarray[currec].key!=K) 
             return -2; 
            } 
            else 
            {
               
                currec=removehelp((BPNode)root.recarray[currec].pointer,K,currec); 
                if(currec<0)           
                    return currec; 
                else if(((BPNode)root.recarray[currec].pointer).numrec!=0) 
                {                     
                 root.recarray[currec].key= ((BPNode)root.recarray[currec].pointer).recarray[0].key; 
                 return -1;            
                } 
            } 
            removerec(root,currec);
            if (root.numrec > 2)          
            {
                return -1;
            }
            else
            {                              
                if ((root.getPrev() == null) && (root.getNext() == null))
                {                         
                    if ((root.numrec == 1) && (!root.isLeaf))
                    {
                        return 0;          
                    }
                    else
                    {
                        return -1;
                    }
                }
                else if (root.getPrev() == null)
                {                          
                    if (root.numrec + root.getNext().numrec <= root.maxSize)
                    {
                        merge_nodes(root, root.getNext());
                        return thispos + 1; 
                    }
                    else
                    {
                        shuffle_nodes(root, root.getNext());
                        return thispos + 1; 
                    }
                }
                else
                {                          
                    if (root.numrec + root.getPrev().numrec <= root.maxSize)
                    {
                        merge_nodes(root.getPrev(), root);
                        return thispos;     
                    }
                    else
                    {
                        shuffle_nodes(root.getPrev(), root);
                        return thispos;     
                    }
                }
            } 
        } 

        void merge_nodes(BPNode left,BPNode right) 
        { 
            for(int i=left.numrec;i<left.numrec+right.numrec;i++) 
            { 
                left.recarray[i].key=right.recarray[i-left.numrec].key; 
                left.recarray[i].pointer=right.recarray[i-left.numrec].pointer; 
            } 
            left.numrec+=right.numrec; 
            right.numrec=0; 
            left.setNext(right.getNext());
            if (right.getNext() != null)
            {
                right.getNext().setPrev(left);
            }
        } 

        void shuffle_nodes(BPNode left,BPNode right) 
        { 
            if(left.numrec>right.numrec) 
            { 
                for(int i=1;i>=0;i--)     
                { 
                    right.recarray[i+1].key=right.recarray[i].key; 
                    right.recarray[i+1].pointer=right.recarray[i].pointer; 
                } 
                right.recarray[0].key=left.recarray[left.numrec-1].key; 
                right.recarray[0].pointer=left.recarray[left.numrec-1].pointer; 
                left.numrec--;right.numrec++; 
            } 
            else 
            { 
                left.recarray[left.numrec].key=right.recarray[0].key; 
                left.recarray[left.numrec].pointer=right.recarray[0].pointer; 
                for(int i=0;i<right.numrec-1;i++) 
                { 
                    right.recarray[i].key=right.recarray[i+1].key; 
                    right.recarray[i].pointer=right.recarray[i+1].pointer; 
                } 
                left.numrec++;right.numrec--; 
            } 
        }
      
        void removerec(BPNode root,int pos) 
        { 
            for(int i=pos;i<root.numrec-1;i++) 
            { 
                root.recarray[i].key=root.recarray[i+1].key; 
                root.recarray[i].pointer=root.recarray[i+1].pointer; 
            } 
            root.numrec--; 
        } 
        #endregion 

        #region Traversal Function Group
        public IEnumerator GetIEnumerator()
        {
            return (IEnumerator)new BPT_Traversalor(this, MinKey);
        }
        #endregion
    }
    #endregion

    #region Class BPT_Traversalor
    public class BPT_Traversalor : IEnumerator
    {
        private BPNode bpn;
        private BPT MyBpt;
        private int StartKey;
        private int CurPos;

        public BPT_Traversalor(BPT Btree,int StartKey)
        {
            MyBpt = Btree;
            this.StartKey = StartKey;
            bpn = Btree.findhelp (Btree.BPTroot , ref this.StartKey);
            this.StartKey = StartKey;
            CurPos = 1;
        }

        object IEnumerator.Current
        {
            get 
            {

                return bpn.recarray[CurPos-1]; 
            }
        }

        bool IEnumerator.MoveNext()
        {
            if (CurPos >= bpn.numrec)
            {
                if (bpn.getNext() != null)
                {
                    bpn = bpn.getNext();
                    CurPos = 0;
                }
                else
                {
                    return false;
                }

            }
            if (bpn.recarray[CurPos].key == 0 && bpn.recarray[CurPos].pointer==null)
            {
                if (bpn.getNext() == null)
                {
                    return false;
                }
                else
                {
                    bpn = bpn.getNext();
                    CurPos = 0;
                }
            }

            CurPos++;
            return true;
        }

        void IEnumerator.Reset()
        {
            int sk = this.StartKey;
            bpn = MyBpt.findhelp(MyBpt.BPTroot, ref this.StartKey);
            this.StartKey = sk;
            CurPos = 1;
        }
    }
    #endregion

} 
