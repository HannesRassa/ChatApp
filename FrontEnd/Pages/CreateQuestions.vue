<template>
  <div class="create-pack-container">
    <!-- Home Button -->
    <button class="home-button" @click="goHome">Home</button>

    <h1>Create Question Pack</h1>

    <!-- Input for Pack Name -->
    <div>
      <label for="packName">Pack Name:</label>
      <input
        type="text"
        id="packName"
        v-model="packName"
        placeholder="Enter pack name"
        required
      />
    </div>

    <!-- Select Number of Questions -->
    <div>
      <label for="numberOfQuestions">Number of Questions:</label>
      <input
        type="number"
        id="numberOfQuestions"
        v-model.number="numberOfQuestions"
        min="1"
        placeholder="Enter number of questions"
        @change="generateQuestionFields"
      />
    </div>

    <!-- Dropdown for Question Inputs -->
    <div v-if="questions.length > 0" class="question-fields">
      <h3>Enter Questions:</h3>
      <div
        v-for="(question, index) in questions"
        :key="index"
        class="question-input"
      >
        <label :for="'question-' + index">Question {{ index + 1 }}:</label>
        <input
          :id="'question-' + index"
          type="text"
          v-model="question.questionText"
          placeholder="Enter question text"
          required
        />
      </div>
    </div>

    <!-- Save Button -->
    <button @click="submitQuestionPack" :disabled="isSubmitting">
      {{ isSubmitting ? "Saving..." : "Save Question Pack" }}
    </button>
  </div>
</template>

<script lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";
import axios from "axios";

export default {
  name: "CreatePack",
  setup() {
    const packName = ref("");
    const numberOfQuestions = ref<number | null>(null);
    const questions = ref<{ questionText: string; packName: string }[]>([]);
    const isSubmitting = ref(false);
    const router = useRouter();

    const generateQuestionFields = () => {
      if (!numberOfQuestions.value || numberOfQuestions.value <= 0) {
        questions.value = [];
        return;
      }

      questions.value = Array.from({ length: numberOfQuestions.value }, () => ({
        questionText: "",
        packName: packName.value,
      }));
    };

    const submitQuestionPack = async () => {
      if (!packName.value.trim()) {
        alert("Pack name is required.");
        return;
      }

      if (questions.value.some((q) => !q.questionText.trim())) {
        alert("All question fields must be filled out.");
        return;
      }

      // Add pack name to each question
      questions.value.forEach((q) => (q.packName = packName.value.trim()));

      try {
        isSubmitting.value = true;
        const response = await axios.post(
          "https://localhost:7269/Backend/Question/Batch",
          questions.value
        );
        alert("Question pack saved successfully!");
        console.log(response.data);
        // Reset form
        packName.value = "";
        numberOfQuestions.value = null;
        questions.value = [];
      } catch (error) {
        console.error("Error saving question pack:", error);
        alert("Failed to save question pack.");
      } finally {
        isSubmitting.value = false;
      }
    };

    const goHome = () => {
      router.push("/");
    };

    return {
      packName,
      numberOfQuestions,
      questions,
      generateQuestionFields,
      submitQuestionPack,
      isSubmitting,
      goHome,
    };
  },
};
</script>

<style scoped>
.create-pack-container {
  max-width: 600px;
  margin: 0 auto;
  padding: 20px;
  background: linear-gradient(135deg, #2c3e50, #4ca1af);
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.page-title {
  font-size: 24px;
  text-align: center;
  margin-bottom: 20px;
  color: #333;
}

.form-group {
  margin-bottom: 15px;
}

.form-label {
  display: block;
  margin-bottom: 5px;
  font-weight: bold;
  color: #555;
}

.form-input {
  width: 100%;
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 4px;
  font-size: 14px;
}

.primary-button {
  display: block;
  width: 100%;
  padding: 10px;
  font-size: 16px;
  font-weight: bold;
  color: #fff;
  background-color: #a607ce;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.primary-button:disabled {
  background-color: #aaa;
  cursor: not-allowed;
}

.question-fields {
  margin-top: 20px;
}

.section-title {
  font-size: 18px;
  margin-bottom: 10px;
  color: #333;
}

.home-button {
  position: absolute;
  top: 10px;
  left: 10px;
  background-color: #34495e;
  color: white;
  padding: 10px 20px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-size: 16px;
}

input {
  width: 100%;
  padding: 8px;
  margin: 8px 0;
  box-sizing: border-box;
}

h1 {
  margin-bottom: 20px;
}
</style>
