# Helpful Information
Here you can find some helpful information about how to use GRD_FSM.

## Any State Block
Every FSM Manager contains an **Any State** block. This block is used to create transitions that can be executed by any state. FSM Manager checks the transitions in the Any State block before the transitions in the current state.

## Transition Order
In the State Properties area, the order of the transitions in the list matters. Transitions are verified in the order they are in the list, so transitions above have them priority. If two transitions have their conditions satisfied at the same time, the transition higher up in the list is executed.

## Changing a State Behaviour Class Name
If you change the name of a State Behaviour class, Unity will lose its reference in serialized objects, breaking all FSM Manager that contain this behaviour. To change the class name avoiding this problem, you can use the MovedFrom attribute. Here is an example showing how to do this:

Before:
```
using GRD.FSM;

namespace YourNamespace
{
	public class OldBehaviourClassName : FSM_StateBehaviour
	{
		// Behaviour code...
	}
}
```
After:
```
using GRD.FSM;
using UnityEngine.Scripting.APIUpdating;

namespace YourNamespace
{
	[MovedFrom(true, sourceAssembly: "Assembly-CSharp", sourceNamespace:"YourNamespace", sourceClassName: "OldBehaviourClassName")]
	public class NewBehaviourClassName : FSM_StateBehaviour
	{
		// Behaviour code...
	}
}
```

After the code is compiled by Unity, all references are updated and you can remove the MovedFrom attribute.
See more about [MovedFromAttribute](https://github.com/Unity-Technologies/UnityCsReference/blob/master/Runtime/Export/Scripting/APIUpdating/UpdatedFromAttribute.cs).