# UnityNotificationManager
A library for Unity Android to use local push notifications.

## How It Works

The functioning is kind of explained here. This is more like a test to see if GitHub reads mermaid.



```mermaid
graph TD

    classDef uno fill:#F5C396 ,stroke:#333,stroke-width:2px;
    classDef due fill:#B0DB43 ,stroke:#333,stroke-width:2px;
    classDef tre fill:#BCE7FD ,stroke:#333,stroke-width:2px;
    classDef qua fill:#5DA9E9 ,stroke:#333,stroke-width:2px;
    classDef cin fill:#23967F ,stroke:#333,stroke-width:2px;
    classDef sei fill:#ED6B86 ,stroke:#333,stroke-width:2px;
    
    subgraph UnityApp [Unity Application]
    	subgraph Unity [Unity]
    		subgraph Other [OtherClasses]
    			Method(Method)
    		end
			subgraph NotificationsManager [Notifications Manager]
				UAdd(Add Notifications)
				UReorder(Reorder Notification)
				URem(Remove Notification)
			end
    	end
    	subgraph Library [Library]
    		q(Notifications Queue)
    		AAdd(Add Notification)
    		AReorder(Reorder Notifications)
    		ARem(Remove Notification)
			subgraph Thread[Thread]
				ThreadPop(Pop Notification)
			end
			
    	end
    end
    
    class UnityApp uno
    class Unity,Library due
    class NotificationsManager,q,AReorder,AAdd,ARem,Other tre
    class UAdd,UReorder,Thread,URem,Method qua
    class ThreadPop cin
    
    UAdd --> AAdd
    UReorder --> AReorder
    AReorder --> q
    AAdd --> q
	URem --> ARem
	ARem --> q
	Method --> UReorder
	Method --> UAdd
	Method --> URem
    q --> ThreadPop
```

