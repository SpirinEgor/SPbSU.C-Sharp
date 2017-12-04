#!/usr/bin/env python
import sys

out_test = [line.strip() for line in open(sys.argv[1], 'r')]
out_gold = [line.strip() for line in open(sys.argv[2], 'r')]
if len(out_test) != len(out_gold):
    print False
    exit(0)
for line_gold in out_gold:
    if not line_gold in out_test:
        print False
        exit(0)
