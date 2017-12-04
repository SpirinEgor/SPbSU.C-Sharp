Testing:
1. Create folder `RedBlackTree` in your repository;
2. Copy `tasks` folder to `RedBlackTree` folder;
3. Create `RedBlackTree.exe` and put it in `RedBlackFolder` folder (if you are using `Rider`, you can find `.exe` file in `<your project>/bin/Release/`);
4. Make pull request.

Interface for tree:
1. `insert key value` ‒ вставить вершину с ключом `key` и значением `value`;
2. `remove key` ‒ удалить вершину с ключом `key`;
3. `find key` ‒ найти вершину с ключом `key`: `#id: null` если такой вершины нет, иначе `#id: value` этой вершины, где `id` порядковый номер соответствующего запроса (нумерация с 0);
4. `exit` ‒ конец ввода.
Пример:
input:
 insert 1 2 
 find 1
 remove 1          
 find 1              
 exit
output:
 #0: 2
 #1: null

