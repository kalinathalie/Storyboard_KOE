with open("file.srt", "r") as file:
	for linha in file.readlines():
		line = linha[0:-1]
		if line.isdigit():
			print(int(line)+74)
		else:
			print(line)