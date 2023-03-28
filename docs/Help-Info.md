# Helpful Information
Here you can find some helpful information about how to use GRD_FSM.

## Any State Block
Every FSM Manager contains an **Any State** block. This block is used to create transitions that can be executed by any state. FSM Manager checks the transitions in the Any State block before the transitions in the current state.

## Transition Order
In the State Properties area, the order of the transitions in the list matters. Transitions are verified in the order they are in the list, so transitions above have them priority. If two transitions have their conditions satisfied at the same time, the transition higher up in the list is executed.