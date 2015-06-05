---

title: Akka.Net - Reactive Programming in C#
class: title-slide

---

title: Akka.Net
subtitle: Reactive Programming
class: big
build_lists: true

Things we'll cover:

- What is Reactive Programming?
- How does Akka.Net fit in?
- Actor's and the Actor System 
- Distributed vs. Local
- Lifecycle Monitoring
- Persistence
- Publisher/Subscriber
- Circuit Breaker
- Deployment

---

title: Reactive Programming
subtitle: What is it?
class: big
build_lists: true

It is NOT:

- Reactive Programming != Imperative Programming
- Reactive Programming != Object-oriented Programming
- Reactive Programming != Functional Programming

BUT:

- Reactive && Object-oriented && Functional && Imperative

---

title: Reactive Programming
subtitle: What is it?
class: big
build_lists: true

It is:

- Software Design Methodology & Programming Paradigm
- Stems from issues encountered in developing Distributed Systems
- Specically from synchronizing events in an ordered way across multiple processes 
- Can be traced back to a specific paper by Leslie Lamport [paper](assets/time-clocks.pdf)
- Expanded to include error handling, scaling and consistency

<footer class="source">source: http://www.lamport.org/</footer>

---

title: Reactive Programming
subtitle: Four Characteristics
class: big
build_lists: true

Four Central Tenenats:

- Responsive: Ability to respond in a timely manner if possible
- Resilient: Ability to handle failure and recover
- Elastic: Ability to scale under varying workloads
- Message Driven: Ability to asychronously communicate between components

![Reactive Traits](/images/reactive-traits.png)

<footer class="source">source: http://www.reactivemanifesto.org/</footer>

---

title: Akka.Net
subtitle: How does this fit?
class: big
build_lists: true

Akka.Net

- is a System for 
- Resilient: Ability to handle failure and recover
- Elastic: Ability to scale under varying workloads
- Message Driven: Ability to asychronously communicate between components

<footer class="source">source: http://www.reactivemanifesto.org/</footer>