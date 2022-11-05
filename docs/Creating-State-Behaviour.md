# Creating a State Behaviour
In State Behaviours we code the behaviour assumed by the machine when it is in a specific state. To create it, just create a class that inherits from FSM_StateBehaviour.

```
using GRD.FSM

[FSM_Behaviour("Player/Idle)]
public class Player_IdleState : FSM_StateBehaviour
{
	// Behaviour code here
}
```

You can use the `FSM_Behaviour` attribute to define how the behaviour will be shown in behaviour selection. Otherwise, the behaviour will be shown with its class name.
![Behaviour Selection](images/BehaviourSelection.jpg)

After assigning a behaviour to a state, the class name is shown with a button to remove it.
![Assigned behaviour(images/BehaviourSelected.jpg)

## Coding the Behaviour
The FSM_StateBehaviour class implements a set of functions that are called by FSM_Manager at specific moments. You can override them to implement your logic and control the machine.
Here you can see all the functions implemented by FSM_StateBehaviour class:

| Name | Description |
| ---- | ----------- |
| Setup(FSM_Manager manager) | The FSM_Manager calls this functions once on Awake. This function receives the FSM_Manager as parameter. |
| OnEnter() | Called when the machine enters the state. |
| OnUpdate() | Called when the machine is in this state during Monobehaviour Update iteration. |
| OnFixedUpdate | Called when the machine is in this state during Monobehaviour FixedUpdate iteration. |
| OnLateUpdate | Called when the machine is in this state during Monobehaviour LateUpdate iteration. |
| OnExit | Called when the machine leaves the state. |
| OnCollisionEnter(Collision collision) | Called by FSM_Manager on its OnCollisionEnter callback |
| OnCollisionStay(Collision collision) | Called by FSM_Manager on its OnCollisionStay callback |
| OnCollisionExit(Collision collision) | Called by FSM_Manager on its OnCollisionExit callback |
| OnCollisionEnter2D(Collision collision) | Called by FSM_Manager on its OnCollisionEnter2D callback |
| OnCollisionStay2D(Collision collision) | Called by FSM_Manager on its OnCollisionStay2D callback |
| OnCollisionExit2D(Collision collision) | Called by FSM_Manager on its OnCollisionExit2D callback |

## Controlling the Finite State Machine
Access to a reference of the FSM_Manager component is important for controlling the machine during its life. State Behaviours can get this reference in Setup function and other classes can do it in many ways, like any other component. Through FSM_Manager we can get and set paramenters, as well as access state properties.

| Name | Description |
| ---- | ----------- |
| GetParameterValue | Gets a FSM parameter value. You can give the desired parameter name or ID. |
| SetInt | Sets a integer parameter value. You can give the parameter name or ID. |
| SetFloat | Sets a float parameter value. You can give the parameter name or ID. |
| SetBool | Sets a boolean parameter value. You can give the parameter name or ID. |
| SetTrigger | Sets a trigger parameter as **True**. You can give the parameter name or ID. |
| GetStateIdByName | Gets a FSM state ID giving its name. |
| GetCurrentStateId | Gets the current state ID. |
| GetCurrentStateName | Gets the current state name. |

You can see the example project to better understand how to implement GRD_FSM in you project. In the [next section](Help-Info.md) I've compiled some helpful information about how to use this asset.