# virtualProcessor

A virtual 8 bit processor executing a simplified assembly language. Supports intrerupts and the instructions are stored in
RAM.

First 64 bytes are in no-intrerupt mode. When intrerupting execution jumps to zero.

## Intrerupt types:
  0: SystemStartup
  1: Clock
  2: Hardware
  3: Software

## Registers:
  R1, R2, R3, R4   - general registers used for arithmetic operations and moving data
  It, Id  - intrerupt type and intrerupt data, registers that store intrerupt related data. Read-only.
  Ip, Pip - instruction pointer, previous intruction pointer, register that contain the location in RAM of the current and previous instruction. Read-only.

## Supported instructions:
  ### Jump <R_new_loc>
    Unconditional jump to the location held in the register <R_new_location>
  
  ### JumpZero <R_new_loc> <R_to_test>
    Jump to value held in <R_new_loc> if value stored in <R_to_test> is zero.
  
  ### JumpPositive <R_new_loc> <R_to_test>
    Jump to value held in <R_new_loc> if value stored in <R_to_test> is positive.
  
  ### JumpNegative <R_new_loc> <R_to_test>
    Jump to value held in <R_new_loc> if value stored in <R_to_test> is negative.
  
  ### MovLiteral <R_dst> <Number>
    Copy literal <Number> value into the register <R_dst>
  
  ### MovRegisters <R_dst> <R_src>
    Copy value stored in <R_src> into <R_dst>
  
  ### MovFromMemory <R_dst> <R_containing_mem_address>
    Copy byte at location pointed by  <R_containing_mem_address> into <R_dst>
  
  ### MovToMemory <R_containing_mem_address> <R_src>
    Copy value stored in <R_src> to location pointed by <R_containing_mem_address>
  
  ### Add <R_res> <R_op1> <R_op2>
    Add <R_op1> to <R_op2> and store result in <R_res>
  
  ### Sub <R_res> <R_op1> <R_op2>
     Subtract <R_op2> from <R_op1> and store result in <R_res>
  
  ### Mul <R_res> <R_op1> <R_op2>
   Multiply <R_op1> to <R_op2> and store result in <R_res>
  
  ### Div <R_res> <R_op1> <R_op2>
    Divide <R_op1> by <R_op2> and store result in <R_res>
  
  ### Mod <R_res> <R_op1> <R_op2>
    Divide <R_op1> to <R_op2> and store the remainder in <R_res>
  
  ### IntreruptClock <R_number_of_cycles> <R_intr_data>
    Try to intrerupt the processor after the number of cycles contained in <R_number_of_cycles> with interupt data (ID) copyed from <R_intr_data>.
  
  ### IntreruptSoft <R_intr_data>
    Send a soft intrerupt (IT will have value 3) with interupt data (ID) copyed from <R_intr_data>. 
  
  
