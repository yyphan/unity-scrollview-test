# Yao Yifan's unity-scrollview-test

## Idea
The basic idea is to have just enough rows to fit the viewport and update each one's data/activeState to 'fake' scrolling. The problem is, how lazy can we be?

My idea has 2 folds:

### 1. When the scroll is slow enough (specifically, scrolled no more than one row height between updates)
Here I only update anything when

a. any row goes out of sight (I hide it)

b. top row goes fully inside viewport (potentially inserting one row above to be new top row)

c. same as b but for bottom row

At other times the rows will flow with its parent (`ScrollView`'s Content) and the code will do nothing.

### 2. When the scroll is fast or when it jumped
No other way I could think of except force setting every row

## Key Decisions
### Not using `VerticalLayoutGroup`
I guess it had to do with the idea explained above, I am no longer able to use `VerticalLayoutGroup` in Content. But since I am only updating a row's position when I enable it for the first time, not using `VerticalLayoutGroup` has turned out to be an optimization

### Using `LinkedList<T>`
Truth be told this is my first time using `LinkedList<T> ` in C#, I was going to write my own Doubly Linked List then I discovered its existence. The idea is I need to access and insert to both top and bottom row (without indexing anything in the middle) inside `ScrollView`'s Content so `LinkedList<T>` made perfect sense.

## Other Optimizations
Removed unused GameObjects in scene and turned off some URP settings

## Results Profiled with 20000 rows (a bit extreme, I know)

### CPU Load / Draw calls reduction
![e1cb1119b4dad8ce59d6ae1bf636113](https://github.com/user-attachments/assets/452bc3cc-e6f8-4e7c-8545-1d378bf7bab7)

![ab4f91ae2c26f1bdc43fd268402c58e](https://github.com/user-attachments/assets/10d03840-0b7b-43d1-af86-e2f7da675967)

### Worst frame when scrolling
![555d78dfe8a45a7c5f190c8014a0416](https://github.com/user-attachments/assets/5ef097d1-07c0-49e0-9097-693c6bdd303f)

![63815b1f31e20c7e8cfafa74518d255](https://github.com/user-attachments/assets/79110414-44ce-407c-919b-7eec2dc99909)
