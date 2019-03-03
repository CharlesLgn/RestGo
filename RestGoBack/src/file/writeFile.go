package file

import (
	"fmt"
	"os"
)

func Log(path string, str string){
	f, err := os.Create(path)
	if err != nil {
		fmt.Println(err)
		return
	}
	_ , err = f.WriteString(str)
	if err != nil {
		fmt.Println(err)
		f.Close()
		return
	}
}
