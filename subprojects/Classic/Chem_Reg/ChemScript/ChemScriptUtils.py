from ChemScript import *


# search solvate table, *solvateSDF*, to check if *m* is a solvate
def isSolvate(m, solvateSDF):
	if m == None:
		return False
	f = False
	reader = SDFileReader(solvateSDF)
	while (reader.readNext()):
		if (m.canonicalCode() == reader.current().canonicalCode()):
			f = True
			break
	return f


# count repeat number for a mol array. e. g. for [A, B, C, A], it returns [ [A, A], [B], [C] ]
def countRepeats(array):
	if array == None or len(array) == 0:
		return []
	elif len(array) == 1:
		return [ [array[0]] ]
	cans = []
	for i in array:
		cans.append(i.canonicalCode())
	ret = []
	for k in range(0, len(cans)):
		if (cans[k] == None):
			continue
		list = []
		ret.append(list)
		list.append(array[k])
		for j in range(k + 1, len(cans)):
			if cans[k] == cans[j]:
				list.append(array[j])
				cans[j] = None
	return ret
	
	
# calculate the greatest common divisor of a and b
def divisor(a, b):
	tmp = max(a, b) % min(a, b)
	if tmp == 0:
		return min(a, b)
	else:
		return divisor(min(a, b), tmp)
		
		
# calculate the greatest common divisor of an array
def greatestCommonDivisor(list):
	if len(list) == 0:
		return 1
	if len(list) == 1:
		return list[0]
	ret = divisor(list[0], list[1])
	for i in range(2, len(list)):
		ret = min(ret, divisor(ret, list[i]))
	return ret


# wrap an array [ [A, A], [B], [C] ] as a single string, so javascript can easily parse it
def wrapMolArray(array, separator):
	if array == None or len(array) == 0:
		return "0" + separator + '' + separator
	nn = []
	for i in range(0, len(array)):
		nn.append(len(array[i]))
	gcd = greatestCommonDivisor(nn)
	s = str(gcd) + separator
	m = None
	for i in range(0, len(array)):
		list = array[i]
		for j in range(0, len(list) / gcd):
			if (m == None):
				m = list[j]
			else:
				m.joinFragments(list[j])
	s += m.cdx(True) + separator
	return s


# CARA salt stripping:
# 1). main + saltA + saltA + saltB + solv1 + solv1 + solv2  --->  main, (saltA + saltA + saltB) : 1, (solv1 + solv1 + solv2) : 1
# 2). main + saltA + saltA + saltB + saltB + solv1 + solv1 + solv2 + solv2  --->  main, (saltA + saltB) : 2, (solv1 + solv2) : 2
def splitSaltsCountRepeat_Str(m, st, solvateSDF, separator):
	tt = m.splitSalt(3, st)
	if tt == None:
		return None
	ss = tt[1].splitFragments()

	salts = []
	solvates = []
	for s in ss:
		if isSolvate(s, solvateSDF):
			solvates.append(s)
		else :
			salts.append(s)

	listSalts = countRepeats(salts)
	listSolvates = countRepeats(solvates)

	retstr = []
	retstr.append(tt[0].cdx(True))
	retstr.append(wrapMolArray(listSalts, separator))
	retstr.append(wrapMolArray(listSolvates, separator))
	return retstr
