Pseudorandom Number Generator
=============================

Here is the Pseudorandom Number Generator written on .NET and the NIST test to check its cryptographic quality.

The generator is based on the three linear feedback shift registers (LFSR):
1. LSFR-1 of length 27 (the tapped bits are 1, 2, 5, 27),
2. LFSR-2 of length 26 (1, 2, 6, 25),
3. LFSR-3 of length 25 (3, 25).

The PRNG uses the following non-linear combining function:
g = x1 ⋅ x3 ⊕ x2 ⊕1,
where x1, x2, x3 are the bits we get after shifting LFSR-1, LFSR-2, LFSR-3.

The generator is tested with the Test for the Longest Run of Ones in a Block). 
The description and the reference data are taken from the NIST specification (goes within the repository) on page 29.

The generator actually appears to fail the test. The explanation of this fact and other details can be found in the supporting report ("Semkin Pete - Report ...").
