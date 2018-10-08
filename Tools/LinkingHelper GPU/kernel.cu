#include <stdio.h>
#include <ctime>
#include <cstdlib>
#include <iostream>
#include <cstdint>
#include <fstream>
#include <stdlib.h>
#include <string>

using namespace std;

struct Test {
	char* Value;
	int Length;
};
struct Label {
	Test* Value;
	int Length;
};

Label* GetItems(int length);
Label* GetItems2(int length);


int GetTime() {
	return clock();
}

__global__ 
void vectorAdd( Label *items, Label *items2, int* result) {
	int i = blockIdx.x * blockDim.x;
	int j = threadIdx.x;
	
	int exists;
	int matches;
	int highestMatch = 0;
	matches = 0;
	exists = 0;
	
	if(items[i].Value[0].Length != items2[j].Value[0].Length)
		return;
	
	result[0] = 5;
	
	exists = 1;
	
	if(items[i].Value[0].Value[0] != items2[j].Value[0].Value[0]) {
		exists = 0;
	}
	//if(items[i].Value[0].Value[1] != items2[j].Value[0].Value[1]) {
	//	exists = 0;
	//}
	
	if(exists == 1){
		matches++;
	}
	
	//matches = matches * 100 / items[i].Length;
	
	highestMatch = matches;
	
	if(result[0] < highestMatch)
		result[0] = highestMatch;
}

#define M 512
int main( int argc, char *argv[]) {
	
	//int length = 1035;
	int length = 1000;
	int length2 = 1000;
	
	Label* items = GetItems(length);
	//Label* items2 = GetItems2(length2);
	Label* items2 = items;//GetItems(length2);
	cout << "Read complete." << endl;
	Label* _items;
	Label* _items2;
	int* _result;
	int* result = new int[1];
	int r = 0;
	
	result[0] = 0;
	
	cout << "Allocating memory on card." << endl;
	
	cudaMalloc( (void**)&_items, sizeof(Label) * length );
	cudaMalloc( (void**)&_items2, sizeof(Label) * length2 );
	cudaMalloc( (void**)&_result, sizeof(int) );
	
	cudaMemcpy( _items, items, length * sizeof(Label), cudaMemcpyHostToDevice );
	cudaMemcpy( _items2, items2, length2 * sizeof(Label), cudaMemcpyHostToDevice );
	cudaMemcpy( _result, result, sizeof(int), cudaMemcpyHostToDevice );
	
	printf("Starting measuring\n");
	int startTime = GetTime();
	
	int exists;
	int matches;
	int highestMatch = 0;
	int w1 = 0;
	int w2 = 0;
	for(int i = 0; i < length; i++){
		highestMatch = 0;
		if(items[i].Length <= 0)
			continue;
		for(int j = 0; j < length; j++){
			if(items2[j].Length <= 0)
				continue;
			matches = 0;
			//for(int w1 = 0; w1 < items[i].Length; w1++){
				exists = 0;
				
				
				//for(int w2 = 0; w2 < items2[j].Length; w2++){
					if(items[i].Value[w1].Length != items2[j].Value[w2].Length)
						continue;
					
					if(items[i].Value[w1].Length == 0)
						continue;
					
					//cout << items[i].Value[w1].Value << ":" << items[i].Value[w1].Length << endl;
					//cout << items2[j].Value[w1].Value << ":" << items2[j].Value[w1].Length << endl;
					
					exists = 1;
					
					/*for(int c = 0; c < sizeof(items[i].Value[w1]); c++) {
						if(items[i].Value[w1][c] != items2[j].Value[w2][c]) {
							exists = 0;
						}
					}*/
					
					//cout << items[i].Value[w1].Value[0] << " " << items2[j].Value[w2].Value[0];
					
					if(items[i].Value[w1].Value[0] != items2[j].Value[w2].Value[0]) {
						exists = 0;
					}
					
				
				
				
					if(exists == 1){
						matches++;
						//cout << " " << exists << " " << matches << endl;
						//break;
					}
				//}
			//}
			
			//matches = matches * 100 / items[i].Length;
			//matches = matches * 100 / 2;
			
			if(matches > highestMatch)
				highestMatch = matches;
		}
	}
	r = highestMatch;
	
	int endTime = GetTime();
	
	printf("CPU: ");
	printf("%d\n", (endTime - startTime));
	
	startTime = GetTime();
	
	vectorAdd<<<length,length2>>>( _items, _items2, _result );
	
	endTime = GetTime();
	
	printf("GPU: ");
	printf("%d\n", (endTime - startTime));
	
	cudaMemcpy( result, _result, sizeof(int), cudaMemcpyDeviceToHost ) ;
	
	cout << r << endl;
	cout << result[0] << endl;
	
	// free the memory allocated on the GPU
	cudaFree( _items );
	cudaFree( _items2 );
	cudaFree( _result );
	
	return 0;
}

Label* GetItems(int length) {
	string line;
	
	Label* items = new Label[length];
	
	ifstream afile;
	afile.open("Test1.txt", ios::in );
	
	int index = 0;
	string* words;	
	while ( getline (afile,line) )
    {
		Label lbl;
		words = new string[1000];
		//char *cstr = new char[line.length() + 1];
		//strcpy(cstr, line.c_str());
		int wordLength = 0;
		int startIndex = 0;
		int wordCount = 0;
		for(int i = 0; i < line.length(); i++) {
			if(i == (line.length() - 1))
				wordLength++;
			
			if(line[i] == ' ' || line[i] == '_' || i == (line.length() - 1)){
				
				if(wordLength == 0 || (startIndex + wordLength) >= line.length())
					continue;
				
				words[wordCount] = line.substr(startIndex, wordLength);
				
				wordLength = 0;
				startIndex= i + 1;
				wordCount++;
				continue;
			}
			wordLength++;
		}
		
		lbl.Value = new Test[wordCount];
		
		for(int i = 0; i < wordCount; i++) {
		
			Test test;
			
			char *cstr = new char[wordLength + 1];
			strcpy(cstr, words[i].c_str());
			
			test.Value = cstr;
			test.Length = words[i].length();
		
			lbl.Value[i] = test;
		}
		
		lbl.Length = wordCount;
		items[index++] = lbl;
    }
	
	afile.close();
	
	return items;
}
Label* GetItems2(int length) {
	string line;
	
	Label* items = new Label[length];
	
	ifstream afile;
	afile.open("Test2.txt", ios::in );
	
	int index = 0;
	string* words;
	while ( getline (afile,line) )
    {
		Label lbl;
		words = new string[1000];
		//char *cstr = new char[line.length() + 1];
		//strcpy(cstr, line.c_str());
		int wordLength = 0;
		int startIndex = 0;
		int wordCount = 0;
		for(int i = 0; i < line.length(); i++) {
			if(i == (line.length() - 1))
				wordLength++;
			
			if(line[i] == ' ' || line[i] == '_' || i == (line.length() - 1)){
				
				if(wordLength == 0 || (startIndex + wordLength) >= line.length())
					continue;
				
				words[wordCount] = line.substr(startIndex, wordLength);
				
				wordLength = 0;
				startIndex= i + 1;
				wordCount++;
				continue;
			}
			wordLength++;
		} 
		
		lbl.Value = new Test[wordCount];
		
		for(int i = 0; i < wordCount; i++) {
		
			Test test;
			
			char *cstr = new char[wordLength + 1];
			strcpy(cstr, words[i].c_str());
			
			test.Value = cstr;
			test.Length = words[i].length();
		
			lbl.Value[i] = test;
		}
		
		lbl.Length = wordCount;
		items[index++] = lbl;
    }
	
	afile.close();
	
	return items;
}