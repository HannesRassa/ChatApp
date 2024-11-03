<template>
  <div>
    <p>Questions</p>
    <ul v-if="questions && questions.length">
      <li v-for="q in questions" :key="q.Id">
        {{ q.questionText }}
      </li>
    </ul>
    <p v-else>No questions available.</p>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const questions = ref([]) // Define questions as a reactive ref
const error = ref(null)    // Define error to store any error messages

// Fetch questions using fetch API on component mount
onMounted(async () => {
  try {
    console.log("Fetching questions..."); // Log when fetch starts

    const response = await fetch("http://localhost:5180/backend/question")
    console.log("Response status:", response.status); // Log response status

    if (!response.ok) throw new Error("Network response was not ok");

    questions.value = await response.json();
    console.log("Fetched questions data:", questions.value); // Log the fetched data

  } catch (err) {
    error.value = err.message;
    console.error("Failed to fetch questions:", err);
  }
})
</script>


<style scoped>
ul {
  list-style-type: disc;
  padding-left: 20px;
}

li {
  margin-bottom: 8px;
}
</style>
